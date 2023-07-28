namespace Fold.Motor.Resources.Resolution;

public class ActionResolution
{
    public Request.ActionRequest? Request { set; get; }
    public CardColor Color { set; get; }
    public Response.GameEndedResponse? GameEndedResponse { set; get; }
    public Response.ActionResponse? OwnerResponse { set; get; }
    public Response.ActionResponse? AllResponse { set; get; }
}