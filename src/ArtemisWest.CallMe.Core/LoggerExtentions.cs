using System;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ArtemisWest.CallMe
{
    public static class LoggerExtentions
    {
        [StringFormatMethod("message")]
        public static void Fatal(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Fatal, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Fatal(this ILogger logger, string message, params object[] args)
        {
            logger.Fatal(null, message);
        }

        [StringFormatMethod("message")]
        public static void Error(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Error, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Error(this ILogger logger, string message, params object[] args)
        {
            logger.Error(null, message, args);
        }

        [StringFormatMethod("message")]
        public static void Info(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Info, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Info(this ILogger logger, string message, params object[] args)
        {
            logger.Info(null, message);
        }

        [StringFormatMethod("message")]
        public static void Warn(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Warn, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Warn(this ILogger logger, string message, params object[] args)
        {
            logger.Warn(null, message);
        }
        
        [StringFormatMethod("message")]
        public static void Debug(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Debug, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Debug(this ILogger logger, string message, params object[] args)
        {
            logger.Trace(null, message);
        }

        [StringFormatMethod("message")]
        public static void Trace(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Trace, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Trace(this ILogger logger, string message, params object[] args)
        {
            logger.Trace(null, message);
        }

        [StringFormatMethod("message")]
        public static void Verbose(this ILogger logger, Exception exception, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            logger.Write(LogLevel.Verbose, formattedMessage, exception);
        }
        [StringFormatMethod("message")]
        public static void Verbose(this ILogger logger, string message, params object[] args)
        {
            logger.Verbose(null, message);
        }
    }
}