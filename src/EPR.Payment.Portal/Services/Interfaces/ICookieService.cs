namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface ICookieService
    {
        void SetCookieAcceptance(bool accept, IRequestCookieCollection cookies, IResponseCookies responseCookies);

        bool HasUserAcceptedCookies(IRequestCookieCollection cookies);
    }
}