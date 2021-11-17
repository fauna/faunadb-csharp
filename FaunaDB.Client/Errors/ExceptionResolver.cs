using System;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Errors
{
    public class ExceptionResolver
    {
        public static FaunaException[] Resolve(QueryErrorResponse response, int httpStatusCode)
        {
            List<FaunaException> exceptions = new List<FaunaException>();
            foreach (var error in response.Errors)
            {
                string[] exceptionPositions = new string[error.Position.Count];
                string[] responsePositions = error.Position.ToArray();
                Array.Copy(responsePositions, exceptionPositions, responsePositions.Length);

                switch (error.Code)
                {
                    case ExceptionCodes.InvalidRef:
                        exceptions.Add(new InvalidRefException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidUrlParameter:
                        exceptions.Add(new InvalidUrlParameterException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidExpression:
                        exceptions.Add(new InvalidExpressionException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InstanceAlreadyExists:
                        exceptions.Add(new InstanceAlreadyExistsException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.ValidationFailed:
                        var failures = error.Failures.Select(v =>
                            "field[" + string.Join(",", v.Field) + "]" + " - " + v.Code + ": " + v.Description).ToList();
                        exceptions.Add(new ValidationFailedException(httpStatusCode, error.Description, exceptionPositions, failures));
                        break;
                    case ExceptionCodes.FeatureNotAvailable:
                        exceptions.Add(new FeatureNotAvailableException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.ValueNotFound:
                        exceptions.Add(new ValueNotFoundException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InstanceNotFound:
                        exceptions.Add(new InstanceNotFoundException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.AuthenticationFailed:
                        exceptions.Add(new AuthenticationFailedException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidArgument:
                        exceptions.Add(new InvalidArgumentException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.TransactionAborted:
                        exceptions.Add(new TransactionAbortedException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidWriteTime:
                        exceptions.Add(new InvalidWriteTimeException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.MissingIdentity:
                        exceptions.Add(new MissingIdentityException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidToken:
                        exceptions.Add(new InvalidTokenException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.CallError:
                        var faunaExceptions = error.Failures.Select(cause =>
                            new FunctionCallException(httpStatusCode, cause.Description, cause.Position.ToArray(),
                                new List<FaunaException>())).ToList();
                        exceptions.Add(new FunctionCallException(httpStatusCode, error.Description, exceptionPositions, faunaExceptions));
                        break;
                    case ExceptionCodes.StackOverflow:
                        exceptions.Add(new StackOverflowException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.PermissionDenied:
                        exceptions.Add(new PermissionDeniedException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InstanceNotUnique:
                        exceptions.Add(new InstanceNotUniqueException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.Unauthorized:
                        var message =
                            error.Description + ". Check that endpoint, schema, port and secret are correct during client’s instantiation";
                        exceptions.Add(new UnauthorizedException(httpStatusCode, message, exceptionPositions));
                        break;
                    default:
                        exceptions.Add(new UnknownException(httpStatusCode, error.Description, exceptionPositions));
                        break;
                }
            }

            return exceptions.ToArray();
        }
    }
}
