using System;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis
{
    public static class SynchronousMessages
    {
        public static async Task<TResult> InvokeAsync<TResult, T>(this IRedisDatabase database, string channel,
            T message)
        {
            var messageId = Guid.NewGuid().ToString("N");
            var responseChannel = new RedisChannel($"RESPONSE::{messageId}", RedisChannel.PatternMode.Literal);

            var completionSource = new TaskCompletionSource<TResult>();

            Task OnProcessResponse(CallbackResponse<TResult> arg)
            {
                if (arg.Error)
                    completionSource.SetException(new RedisResponseException(arg.ErrorMessage));
                else
                    completionSource.SetResult(arg.Payload);
                return Task.CompletedTask;
            }

            await database.SubscribeAsync<CallbackResponse<TResult>>(responseChannel, OnProcessResponse);

            try
            {
                await database.PublishAsync(channel, new CallbackMessage<T>(message, responseChannel.ToString()));
                return await completionSource.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
            }
            finally
            {
                await database.UnsubscribeAsync<CallbackResponse<TResult>>(responseChannel, OnProcessResponse);
            }
        }

        public static Task InvokeAsync<T>(this IRedisDatabase database, string channel, T message)
        {
            return InvokeAsync<object, T>(database, channel, message);
        }
    }

    public class CallbackMessage<T>
    {
        public CallbackMessage(T payload, string callbackChannel)
        {
            Payload = payload;
            CallbackChannel = callbackChannel;
        }

        public T Payload { get; }

        public string CallbackChannel { get; }
    }

#pragma warning disable 8618
    public class CallbackResponse<T>
    {
        public T Payload { get; set; }

        public bool Error { get; set; }

        public string? ErrorMessage { get; set; }
    }
#pragma warning restore 8618
}