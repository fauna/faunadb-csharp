using System;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Errors
{
    public class ExceptionResolver
    {
        public static FaunaException[] Resolve(QueryErrorResponse responce, int httpStatusCode)
        {
            List<FaunaException> exceptions = new List<FaunaException>();
            foreach (var error in responce.Errors)
            {
                string[] exceptionPositions = new string[error.Position.Count];
                string[] responsePosition = error.Position.ToArray();
                Array.Copy(responsePosition, exceptionPositions, responsePosition.Length);

                switch (error.Code)
                {
                    case ExceptionCodes.InvaliRef:
                        exceptions.Add(new InvalidRef(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidUrlParameter:
                        exceptions.Add(new InvalidUrlParameter(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidExpression:
                        exceptions.Add(new InvalidExpression(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.SchemaNotFound:
                        exceptions.Add(new SchemaNotFound(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InstanceAlreadyExists:
                        exceptions.Add(new InstanceAlreadyExists(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.ValidationFailed:
                        exceptions.Add(new ValidationFailed(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.FeatureNotAvailable:
                        exceptions.Add(new FeatureNotAvailable(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.ValueNotFound:
                        exceptions.Add(new ValueNotFound(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InstanceNotFound:
                        exceptions.Add(new InstanceNotFound(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.AuthenticationFailed:
                        exceptions.Add(new AuthenticationFailed(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidArgument:
                        exceptions.Add(new InvalidArgument(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.TransactionAborted:
                        exceptions.Add(new TransactionAborted(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidWriteTime:
                        exceptions.Add(new InvalidWriteTime(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.MissingIdentity:
                        exceptions.Add(new MissingIdentity(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidScope:
                        exceptions.Add(new InvalidScope(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidToken:
                        exceptions.Add(new InvalidToken(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.CallError:
                        exceptions.Add(new CallError(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.StackOverflow:
                        exceptions.Add(new StackOverflow(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.PermissionDenied:
                        exceptions.Add(new PermissionDenied(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InvalidObjectInContainer:
                        exceptions.Add(new InvalidObjectInContainer(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.MoveDatabaseError:
                        exceptions.Add(new MoveDatabaseError(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.RecoveryFailed:
                        exceptions.Add(new RecoveryFailed(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.InstanceNotUnique:
                        exceptions.Add(new InstanceNotUnique(httpStatusCode, error.Description, exceptionPositions));
                        break;
                    case ExceptionCodes.Unauthorized:
                        exceptions.Add(new Unauthorized(httpStatusCode, error.Description, exceptionPositions));
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
