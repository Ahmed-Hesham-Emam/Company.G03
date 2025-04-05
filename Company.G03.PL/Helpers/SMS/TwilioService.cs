using Company.G03.PL.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Company.G03.PL.Helpers.SMS
    {
    public class TwilioService(IOptions<TwilioSettings> _options) : ITwilioService
        {
        public MessageResource SendMessage(Sms sms)
            {

            //Initialize connection

            TwilioClient.Init(_options.Value.AccountSid, _options.Value.AuthToken);

            //Construct message

            var message = MessageResource.Create(
                body: sms.body,
                to: sms.To,
                from: new Twilio.Types.PhoneNumber(_options.Value.PhoneNumber)
            );

            //Return message resource

            return message;

            }
        }
    }
