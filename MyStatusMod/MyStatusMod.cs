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
        public override string GetVersion() => "v1.1.5";
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
            Rigidbody2D rb = HeroController.instance.GetComponent<Rigidbody2D>();
            Vector2 playerVelocity = rb != null ? rb.velocity : Vector2.zero;

            string heroState = "Unknown"; // ! Problems here, which state describes the hero's current state?
            // var fsms = HeroController.instance.GetComponents<PlayMakerFSM>();
            // foreach (var fsm in fsms)
            // {
            //     // Log($"[FSM] Found: {fsm.FsmName} | Active State: {fsm.Fsm?.ActiveStateName}");
            //     if (fsm.FsmName == "ProxyFSM")
            //     {
            //         heroState = fsm.Fsm.ActiveStateName;
            //         break;
            //     }
            // }

            Vector3 bossPos = Vector3.zero;
            Vector2 bossSize = Vector2.zero;
            Vector2 bossVelocity = Vector2.zero;
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

                Rigidbody2D bossRb = currentBoss.GetComponent<Rigidbody2D>();
                bossVelocity = bossRb != null ? bossRb.velocity : Vector2.zero;
            }

            // 更新状态
            server.UpdateStatus(hp, soul, bossHp, playerPos, playerSize, bossPos, bossSize,
                                playerVelocity, bossVelocity, heroState);
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
        private Vector2 playerVelocity;
        private Vector2 bossVelocity;
        private string heroFSMState;
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
                                Vector3 bossPos, Vector2 bossSize, Vector2 playerVelocity, Vector2 bossVelocity,
                                string heroFSMState)
        {
            this.health = hp;
            this.soul = soul;
            this.bossHealth = bossHp;
            this.playerPosition = playerPos;
            this.playerSize = playerSize;
            this.bossPosition = bossPos;
            this.bossSize = bossSize;
            this.playerVelocity = playerVelocity;
            this.bossVelocity = bossVelocity;
            this.heroFSMState = heroFSMState;
        }

        private string GetStatusJson()
        {
            // 帮助函数：避免 NaN/Infinity 导致 JSON 错误
            string FormatFloat(float value)
            {
                return (float.IsNaN(value) || float.IsInfinity(value)) ? "0.00" : value.ToString("F2");
            }

            return $"{{" +
                $"\"health\":{health}," +
                $"\"soul\":{soul}," +
                $"\"bossHealth\":{bossHealth}," +
                $"\"fsmState\":\"{heroFSMState}\"," +
                $"\"player\":{{" +
                    $"\"x\":{FormatFloat(playerPosition.x)}," +
                    $"\"y\":{FormatFloat(playerPosition.y)}," +
                    $"\"width\":{FormatFloat(playerSize.x)}," +
                    $"\"height\":{FormatFloat(playerSize.y)}," +
                    $"\"velocity\":{{" +
                        $"\"x\":{FormatFloat(playerVelocity.x)}," +
                        $"\"y\":{FormatFloat(playerVelocity.y)}" +
                    $"}}" +
                $"}}," +
                $"\"boss\":{{" +
                    $"\"x\":{FormatFloat(bossPosition.x)}," +
                    $"\"y\":{FormatFloat(bossPosition.y)}," +
                    $"\"width\":{FormatFloat(bossSize.x)}," +
                    $"\"height\":{FormatFloat(bossSize.y)}," +
                    $"\"velocity\":{{" +
                        $"\"x\":{FormatFloat(bossVelocity.x)}," +
                        $"\"y\":{FormatFloat(bossVelocity.y)}" +
                    $"}}" +
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
