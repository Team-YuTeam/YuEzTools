using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace YuEzTools.Helpers
{
    public class QQHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string baseUrl = "https://yuet.yuqz.dpdns.org/";

        /// <summary>
        /// 添加或更新房间信息（调用 /v1 接口）
        /// </summary>
        /// <returns>HTTP 状态码，成功为 200/201，失败为对应错误码，网络异常返回 -1</returns>
        public static int AddRoom(
            string roomCode,
            int roomNumberNow,
            int roomMaxNum,
            string roomRegion,
            string roomSender,
            string roomHost,
            int maxImpostor,
            string modFrom)
        {
            try
            {
                var payload = new
                {
                    RoomCode = roomCode,
                    roomNumberNow,
                    roomMaxNum,
                    roomRegion,
                    roomSender,
                    roomHost,
                    MaxImpostor = maxImpostor,
                    ModFrom = modFrom
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = baseUrl.TrimEnd('/') + "/v1";
                var response = _httpClient.PostAsync(url, content).Result;
                return (int)response.StatusCode;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 删除房间（调用 /v2 接口）
        /// </summary>
        /// <returns>HTTP 状态码，成功为 200，失败为对应错误码，网络异常返回 -1</returns>
        public static int DeleteRoom(
            string roomCode,
            string roomRegion,
            string roomSender,
            int DeleteReason // 原因请填写AU官方DisconnectReasons.cs中的数字
            )
        {
            try
            {
                var payload = new
                {
                    RoomCode = roomCode,
                    roomRegion,
                    roomSender,
                    DeleteReason
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = baseUrl.TrimEnd('/') + "/v2";
                var response = _httpClient.PostAsync(url, content).Result;
                return (int)response.StatusCode;
            }
            catch
            {
                return -1;
            }
        }
    }
}