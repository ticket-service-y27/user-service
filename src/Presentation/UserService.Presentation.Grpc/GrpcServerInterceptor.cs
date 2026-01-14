using Grpc.Core;
using Grpc.Core.Interceptors;
using UserService.Application.Abstractions.Persistence.Exceptions;
using UserService.Application.Exceptions;

namespace UserService.Presentation.Grpc;

public class GrpcServerInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (CreateEntityException e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
        catch (NotFoundException e)
        {
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
        catch (AlreadyExistsException e)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
        }
        catch (FieldValidationException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
        catch (InvalidAuthorizeException e)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, e.Message));
        }
        catch (UserBlockedException e)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, e.Message));
        }
        catch (UserAlreadyUnblockedException e)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
        }
        catch (UserAlreadyBlockedException e)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
        }
        catch (ActionOnMainAdminException e)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
        catch (Exception e)
        {
            throw new RpcException(new Status(
                StatusCode.Internal,
                detail: $"Возникла непредвиденная ошибка: {e.Message}"));
        }
    }
}