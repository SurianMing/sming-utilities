using System.Linq.Expressions;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SmingCode.Utilities.ServiceApiClient;

internal class ApiClient<TService>(
    HttpClient _httpClient,
    ApiClientConfiguration<TService> _apiClientConfiguration,
    IEnumerable<IApiClientSendMiddleware> _apiClientSendMiddlewares,
    IServiceProvider _serviceProvider,
    ILogger<ApiClient<TService>> _logger
) : IServiceApiClient<TService> where TService : class
{
    public HttpClient HttpClient => _httpClient;

    public async Task Post(
        string relativeUrl
    ) => await ProcessSendAndCheckSuccessfulResponse<NoResponse>(
        HttpMethod.Post,
        relativeUrl
    );

    public async Task<TResult> Post<TRequest, TResult>(
        string relativeUrl,
        TRequest request,
        HeaderEntryCollection? headers = null
    ) where TRequest : notnull where TResult : notnull
        => await ProcessSendAndCheckSuccessfulResponse<TRequest, TResult>(
            HttpMethod.Post,
            relativeUrl,
            request,
            headers
        );

    public async Task<TResponse> Get<TResponse>(
        string relativeUrl
    ) where TResponse : notnull
        => await ProcessSendAndCheckSuccessfulResponse<TResponse>(
            HttpMethod.Get,
            relativeUrl
        );

    private async Task<TResponse> ProcessSendAndCheckSuccessfulResponse<TResponse>(
        HttpMethod httpMethod,
        string targetUrl,
        HeaderEntryCollection? messageHeaders = null
    ) where TResponse : notnull
        => await ProcessSendAndCheckSuccessfulResponse<NoBody, TResponse>(
            httpMethod,
            targetUrl,
            new NoBody(),
            messageHeaders
        );

    private async Func<ApiClientSendContext<TBody, TResponse>, TResponse> BuildPipeline<TBody, TResponse>(

    ) where TBody : notnull where TResponse : notnull
    {
        Func<ApiClientSendContext<TBody, TResponse>, Task<TResponse>> finalStage = async (context) =>
        {
            var response = await SendAndCheckSuccessfulResponse<TResponse>(
                context.HttpMethod,
                context.TargetUrl,
                context.MessageHeaders,
                typeof(TBody) == typeof(NoBody)
                    ? null
                    : new RequestBody<TBody>(
                        context.Body,
                        _apiClientConfiguration.JsonSerializerOptions
                    )
            );

            return response;
        };

        var contextMethodType = typeof(ApiClientSendContext<TBody, TResponse>);
        foreach (var middleware in _apiClientSendMiddlewares)
        {
            var handleMethod = middleware.GetType().GetMethod("HandleAsync")
                ?? throw new InvalidOperationException(
                    "Attempt to inject ApiClientSend middleware, but injected class does not contain a HandleAsync method."
                );

            var handleMethodParams = handleMethod.GetParameters();
            if (handleMethodParams.All(
                param => param.ParameterType == contextMethodType
                    && param.Name == "context"))
            {
                throw new InvalidOperationException(
                    $"Attempt to inject ApiClientSend middleware, but injected class's HandleAsync method does not take a context parameter of type {contextMethodType.Name}."
                );
            }

            ParameterExpression[] parameterExpressions = [];
            foreach (var param in handleMethodParams)
            {
                var newParamExpression = param.ParameterType == contextMethodType
                    ? 
            }
        }
    }

    private async Task<TResponse> ProcessSendAndCheckSuccessfulResponse<TBody, TResponse>(
        HttpMethod httpMethod,
        string targetUrl,
        TBody body,
        HeaderEntryCollection? messageHeaders = null
    ) where TBody : notnull where TResponse : notnull
    {
        var apiClientSendContext = new ApiClientSendContext<TBody, TResponse>(
            httpMethod,
            targetUrl,
            body,
            messageHeaders ?? [],
            _serviceProvider
        );

        var sendDelegate = new ApiClientSendDelegate<TBody, TResponse>(async (apiClientSendContext) =>
        {
            var response = await SendAndCheckSuccessfulResponse<TResponse>(
                apiClientSendContext.HttpMethod,
                apiClientSendContext.TargetUrl,
                apiClientSendContext.MessageHeaders,
                typeof(TBody) == typeof(NoBody)
                    ? null
                    : new RequestBody<TBody>(
                        apiClientSendContext.Body,
                        _apiClientConfiguration.JsonSerializerOptions
                    )
            );

            return response;
        });

        foreach (var apiClientSendMiddleware in _apiClientSendMiddlewares.Reverse())
        {
            var newDelegateHandler = new ApiClientSendDelegateHandler<TBody, TResponse>(
                sendDelegate
            );

            sendDelegate = new ApiClientSendDelegate<TBody, TResponse>(async (apiClientSendContext) =>
                await apiClientSendMiddleware.HandleAsync(
                    apiClientSendContext,
                    newDelegateHandler
                )
            );
        }

        return await sendDelegate(apiClientSendContext);
    }

    private async Task<TResponse> SendAndCheckSuccessfulResponse<TResponse>(
        HttpMethod httpMethod,
        string relativeUrl,
        HeaderEntryCollection? headers = null,
        IHttpRequestMessageBodyDetail? bodyDetail = null
    ) where TResponse : notnull
    {
        var fullUri = GetFullUri(relativeUrl);

        HttpRequestMessageDetail requestMessageDetail = new(
            httpMethod,
            fullUri,
            headers,
            bodyDetail
        );
        LogOutgoingRequest(requestMessageDetail);

        try
        {
            var response = await _httpClient.SendAsync(requestMessageDetail.HttpRequestMessage);
            await CheckAndLogResponse(fullUri, response);

            return typeof(TResponse) == typeof(NoResponse)
                ? (TResponse)Convert.ChangeType(new NoResponse(), typeof(TResponse))
                : await response.Content.ReadFromJsonAsync<TResponse>()
                    ?? throw new ServiceApiClientException(
                        _apiClientConfiguration.ServiceDisplayName,
                        $"Could not convert api response into the required type - {typeof(TResponse).Name}."
                    );
        }
        catch (ServiceApiClientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LogHttpClientCallException(ex, fullUri);

            throw new ServiceApiClientException(
                _apiClientConfiguration.ServiceDisplayName,
                $"Unable to process call to {fullUri}",
                ex
            );
        }
    }

    private string GetFullUri(string relativeUrl)
        => $"{HttpClient.BaseAddress}{relativeUrl.TrimStart('/')}";

    private void LogOutgoingRequest(HttpRequestMessageDetail requestMessageDetail)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Service Api Client targeting service {TargetServiceName} sending request {RequestDetail} - {TraceType}",
                _apiClientConfiguration.ServiceDisplayName,
                requestMessageDetail.LogDetail,
                Constants.UTILITY_TRACE_TYPE
            );
        }
    }

    private async Task CheckAndLogResponse(
        string targetUrl,
        HttpResponseMessage responseMessage,
        bool throwIfUnsuccessful = true
    )
    {
        if (!responseMessage.IsSuccessStatusCode)
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            _logger.LogError(
                "Service Api Client targeting service {TargetServiceName} received unsuccessful response ({StatusCode}) with details: {ResponseContent} - {TraceType}",
                _apiClientConfiguration.ServiceDisplayName,
                responseMessage.StatusCode,
                responseContent,
                Constants.UTILITY_TRACE_TYPE
            );

            if (throwIfUnsuccessful)
            {
                throw new ServiceApiClientException(
                    _apiClientConfiguration.ServiceDisplayName,
                    $"Service Api Client targeting service {_apiClientConfiguration.ServiceDisplayName} received unsuccessful response ({responseMessage.StatusCode}) for call to {targetUrl}.",
                    _responseMessage: responseMessage
                );
            }
        }
        else if (_logger.IsEnabled(LogLevel.Trace))
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            var responseDetails = new
            {
                StatusCode = responseMessage.StatusCode.ToString(),
                Headers = responseMessage.Headers.Select(header =>
                    $"{header.Key}: {string.Join(';', header.Value)}"
                ).ToList(),
                Content = responseContent
            };

            _logger.LogTrace(
                "Service Api Client targeting service {TargetServiceName} received response with details: {ResponseDetails} - {TraceType}",
                _apiClientConfiguration.ServiceDisplayName,
                responseDetails,
                Constants.UTILITY_TRACE_TYPE
            );
        }
    }

    private void LogHttpClientCallException(
        Exception ex,
        string fullUri
    )
    {
        _logger.LogError(
            ex,
            "Service Api Client targeting service {TargetServiceName} at url {TargetUrl} failed - {TraceType}",
            _apiClientConfiguration.ServiceDisplayName,
            fullUri,
            Constants.UTILITY_TRACE_TYPE
        );
    }
}
