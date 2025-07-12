import requests
import time

start_time = time.time()

def fetch_status():
    try:
        elapsed = time.time() - start_time
        res = requests.get("http://localhost:8081/status/", timeout=1.0)
        data = res.json()

        health = data["health"]
        soul = data["soul"]
        boss_hp = data["bossHealth"]
        fsm_state = data.get("fsmState", "Unknown")

        player = data.get("player", {})
        boss = data.get("boss", {})

        print("="*60)
        print(f"[{elapsed:7.2f} s] Hollow Knight Game Status")
        print(f"FSM State: {fsm_state}")
        print(f"Player HP: {health}")
        print(f"Player Soul: {soul}")
        print(f"Boss HP: {boss_hp}")

        print(f"Player Pos: ({player.get('x', 0):.2f}, {player.get('y', 0):.2f}) "
              f"Size: ({player.get('width', 0):.2f}, {player.get('height', 0):.2f}) "
              f"Vel: ({player['velocity']['x']:.2f}, {player['velocity']['y']:.2f})")

        print(f"Boss   Pos: ({boss.get('x', 0):.2f}, {boss.get('y', 0):.2f}) "
              f"Size: ({boss.get('width', 0):.2f}, {boss.get('height', 0):.2f}) "
              f"Vel: ({boss['velocity']['x']:.2f}, {boss['velocity']['y']:.2f})")
        
        input_state = data.get("input", {})
        pressed_keys = [k for k, v in input_state.items() if v]
        print(f"Keys Pressed: {pressed_keys}")

    except Exception as e:
        print("Error fetching status:", e)

if __name__ == "__main__":
    print("🔄 Polling Hollow Knight status server...")
    while True:
        fetch_status()
        time.sleep(0.4)
