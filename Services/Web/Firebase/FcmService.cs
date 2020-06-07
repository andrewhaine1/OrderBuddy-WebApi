using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Web.Firebase
{
    public class FcmService : IFcmService
    {
        private async Task<bool> NotifyAsync(string to, string title, string body, int orderId, string status)
        {
            // Get the server key from FCM console
            var serverKey = string.Format("key={0}",
                "AAAAFu1clOQ:APA91bH1-H2t7_uiTim2h9ulA" +
                "v2zMiRkq-rolOuU5VE3gtlNTtZzRZ7sfMxGlQk" +
                "jZ4znunh4n025fYcWrUBnCTy-dbTlLXyMJvcWLu" +
                "-6ffl8vQVgUIDzmBrzDic1EW10wFlky9-23fDG");

            // Get the sender id from FCM console
            //var senderId = string.Format("id={0}", "Your sender id - use app config");

            var payload = new
            {
                to, // Recipient device token
                notification = new { 
                    title, 
                    body,
		    color = "#000000",
                    sound = "default",
                    click_action = "FCM_PLUGIN_ACTIVITY"
                },
                data = new { OrderId = orderId, Status = status }
            };

            // Using Newtonsoft.Json
            var jsonBody = JsonConvert.SerializeObject(payload);

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
            {
                httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                httpRequest.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.SendAsync(httpRequest);

                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // Notify Placed - For owner
        public async Task<bool> NotifyOrderPlacedAsync(string to, string fullName, int orderId)
        {
            var title = "Order Placed";
            var body = $"An order has been placed by {fullName}";
            return await NotifyAsync(to, title, body, orderId, status: "placed");
        }

        // Notify Accepted - For user
        public async Task<bool> NotifyOrderAcceptedAsync(string to, string restaurantName, int orderId)
        {
            var title = "Order Accepted";
            var body = $"Your order with {restaurantName} has been been accepted.";
            return await NotifyAsync(to, title, body, orderId, status: "accepted");
        }

        // Notify Rejected - For user
        public async Task<bool> NotifyOrderRejectedAsync(string to, string restaurantName, int orderId)
        {
            var title = "Order Rejected";
            var body = $"Your order with {restaurantName} has been been rejected.";
            return await NotifyAsync(to, title, body, orderId, status: "rejected");
        }

        // Notify Ready - For user
        public async Task<bool> NotifyOrderReadyAsync(string to, string restaurantName, int orderId)
        {
            var title = "Order Ready";
            var body = $"Your order with {restaurantName} is ready for collection.";
            return await NotifyAsync(to, title, body, orderId, status: "ready");
        }

        // Notify Cancelled - For user
        public async Task<bool> NotifyOrderCancelledAsync(string to, string restaurantName, int orderId)
        {
            var title = "Order Cancelled";
            var body = $"Your order with {restaurantName} has been been cancelled.";
            return await NotifyAsync(to, title, body, orderId, status: "cancelled");
        }

        // Notify Completed - For user
        public async Task<bool> NotifyOrderCompletedAsync(string to, string restaurantName, int orderId)
        {
            var title = "Order Completed";
            var body = $"Your order with {restaurantName} has been been completed.";
            return await NotifyAsync(to, title, body, orderId, status: "completed");
        }
    }
}
