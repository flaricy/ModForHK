using Modding;
using UnityEngine;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;

namespace MyStatusMod
{
    public class MyStatusMod : Mod
    {
        public override string GetVersion() => "v1.1.3";
        private StatusServer server;

        public override void Initialize()
        {
            Log("Status Mod Initialized");
            server = new StatusServer();
            server.Start();
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }

        private HealthManager currentBoss = null;

        private void OnHeroUpdate()
        {
            int hp = PlayerData.instance.GetInt("health");
            int soul = PlayerData.instance.GetInt("MPCharge");

            Vector3 playerPos = HeroController.instance.transform.position;
            BoxCollider2D pc = HeroController.instance.GetComponent<BoxCollider2D>();
            Vector2 playerSize = pc != null ? pc.size : Vector2.zero;

            Vector3 bossPos = Vector3.zero;
            Vector2 bossSize = Vector2.zero;
            int bossHp = -1;

            // 尝试识别 boss
            if (currentBoss == null)
            {
                foreach (var hm in GameObject.FindObjectsOfType<HealthManager>())
                {
                    if (hm.hp >= 500)  // 你可以根据需要调整阈值
                    {
                        currentBoss = hm;
                        Log($"Boss Detected: {hm.name}");
                        break;
                    }
                }
            }

            // 如果找到了 boss，安全地访问其信息
            if (currentBoss != null)
            {
                bossHp = currentBoss.hp;
                bossPos = currentBoss.transform.position;

                BoxCollider2D bc = currentBoss.GetComponent<BoxCollider2D>();
                bossSize = bc != null ? bc.size : Vector2.zero;
            }

            // 更新状态
            server.UpdateStatus(hp, soul, bossHp, playerPos, playerSize, bossPos, bossSize);
        }

    }

    public class StatusServer
    {
        private HttpListener listener;
        private Thread serverThread;

        private int health, soul, bossHealth;
        private Vector3 playerPosition;
        private Vector2 playerSize;
        private Vector3 bossPosition;
        private Vector2 bossSize;
        public void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8081/status/");
            listener.Start();

            serverThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        var context = listener.GetContext();
                        var response = context.Response;
                        var json = GetStatusJson();
                        var buffer = Encoding.UTF8.GetBytes(json);

                        response.ContentType = "application/json";
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        response.OutputStream.Close();
                    }
                    catch { /* ignore errors */ }
                }
            });

            serverThread.IsBackground = true;
            serverThread.Start();
        }

        public void UpdateStatus(int hp, int soul, int bossHp, Vector3 playerPos, Vector2 playerSize,
                                Vector3 bossPos, Vector2 bossSize)
        {
            this.health = hp;
            this.soul = soul;
            this.bossHealth = bossHp;
            this.playerPosition = playerPos;
            this.playerSize = playerSize;
            this.bossPosition = bossPos;
            this.bossSize = bossSize;
        }

        private string GetStatusJson()
        {
            return $"{{" +
                $"\"health\":{health}," +
                $"\"soul\":{soul}," +
                $"\"bossHealth\":{bossHealth}," +
                $"\"player\":{{" +
                    $"\"x\":{playerPosition.x:F2},\"y\":{playerPosition.y:F2}," +
                    $"\"width\":{playerSize.x:F2},\"height\":{playerSize.y:F2}" +
                $"}}," +
                $"\"boss\":{{" +
                    $"\"x\":{bossPosition.x:F2},\"y\":{bossPosition.y:F2}," +
                    $"\"width\":{bossSize.x:F2},\"height\":{bossSize.y:F2}" +
                $"}}" +
                $"}}";
        }

        public void Stop()
        {
            listener?.Stop();
            serverThread?.Abort();
        }
    }
}
