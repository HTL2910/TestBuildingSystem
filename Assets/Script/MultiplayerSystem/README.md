# Universal Multiplayer System

Hệ thống multiplayer hoàn chỉnh cho Unity với Photon PUN 2, bao gồm quản lý phòng, networking cho player/enemy, hệ thống health, AI và UI components.

## 🚀 Tính năng

- ✅ **Photon PUN 2 Integration** - Kết nối multiplayer hoàn chỉnh
- ✅ **Room Management** - Tạo/join phòng với settings tùy chỉnh
- ✅ **Player Networking** - Movement, combat, health synchronization
- ✅ **Enemy AI** - AI enemies với networking
- ✅ **Health System** - Health bars và damage system
- ✅ **UI System** - Menu, lobby, game UI hoàn chỉnh
- ✅ **Game Management** - Game states, spawning, scoring

## 📋 Yêu cầu

- Unity 2019.4+
- Photon PUN 2 (đã cài đặt)
- AppID Photon (đã cấu hình: `your appID`)

## 🛠️ Setup nhanh

### 1. Tạo MultiplayerManager trong Scene

```csharp
// Tự động setup khi chạy game
GameObject managerGO = new GameObject("MultiplayerManager");
MultiplayerManager manager = managerGO.AddComponent<MultiplayerManager>();
```

Hoặc sử dụng script `MultiplayerManagerSetup`:
- Thêm component `MultiplayerManagerSetup` vào GameObject bất kỳ
- Chọn player prefab trong inspector
- Script sẽ tự động tạo MultiplayerManager

### 2. Tạo Player Prefab

1. Tạo GameObject mới
2. Thêm component `PlayerPrefabSetup`
3. Script sẽ tự động thêm tất cả components cần thiết:
   - `PhotonView`
   - `NetworkPlayer`
   - `Rigidbody`
   - `Collider`
4. Tạo prefab từ GameObject này

### 3. Tạo Enemy Prefab

1. Tạo GameObject mới
2. Thêm component `EnemyPrefabSetup`
3. Script sẽ tự động thêm tất cả components cần thiết:
   - `PhotonView`
   - `NetworkEnemy`
   - `Rigidbody`
   - `Collider`
   - `Animator` (nếu có)
4. Tạo prefab từ GameObject này

### 4. Setup UI

1. Tạo Canvas với UI panels:
   - Main Menu Panel
   - Connecting Panel
   - Lobby Panel
   - Game Panel
   - Settings Panel

2. Thêm component `MultiplayerUIManager` vào Canvas
3. Assign tất cả UI elements trong inspector

### 5. Setup Game Manager

1. Tạo GameObject "GameManager"
2. Thêm component `MultiplayerGameManager`
3. Assign player/enemy prefabs và spawn points

## 🎮 Sử dụng

### Kết nối và tạo phòng

```csharp
// Kết nối server
MultiplayerManager.Instance.ConnectToServer();

// Tạo phòng
RoomSettings settings = new RoomSettings("MyRoom", 4);
MultiplayerManager.Instance.CreateRoom("MyRoom", settings);

// Join phòng ngẫu nhiên
MultiplayerManager.Instance.JoinRandomRoom();
```

### Player Networking

```csharp
// Spawn player
MultiplayerManager.Instance.SpawnPlayer();

// Take damage
player.TakeDamage(25f);

// Add score
player.AddScore(100);
```

### Enemy AI

```csharp
// Enemy sẽ tự động:
// - Tìm player gần nhất
// - Chase và attack
// - Sync position qua network
// - Respawn sau khi chết
```

## 📁 Cấu trúc thư mục

```
MultiplayerSystem/
├── Core/
│   ├── MultiplayerManager.cs          # Quản lý kết nối và phòng
│   ├── MultiplayerManagerSetup.cs     # Auto setup MultiplayerManager
│   └── MultiplayerGameManager.cs      # Quản lý gameplay
├── Player/
│   ├── NetworkPlayer.cs               # Player networking
│   └── PlayerPrefabSetup.cs           # Auto setup player prefab
├── Enemy/
│   ├── NetworkEnemy.cs                # Enemy AI và networking
│   └── EnemyPrefabSetup.cs            # Auto setup enemy prefab
├── Data/
│   ├── PlayerData.cs                  # Player data structure
│   └── RoomSettings.cs                # Room settings
└── UI/
    ├── HealthBar.cs                   # Health bar component
    └── MultiplayerUIManager.cs        # UI management
```

## 🔧 Cấu hình

### Photon Settings

AppID đã được cấu hình trong `PhotonServerSettings.asset`:
- **AppIdRealtime**: `a3ea1293-81a2-4908-b4d6-e08319c6a018`
- **Region**: Korea (kr)
- **Protocol**: UDP
- **Auto Sync Scene**: Enabled

### Game Settings

Có thể tùy chỉnh trong `MultiplayerGameManager`:
- `gameStartDelay`: 3f
- `respawnDelay`: 3f
- `maxEnemies`: 10
- `enemySpawnInterval`: 5f
- `maxGameTime`: 300 (5 phút)

## 🎯 Game Modes

Hệ thống hỗ trợ nhiều game modes:

### Deathmatch
```csharp
RoomSettings settings = RoomSettings.CreateDeathmatch(4);
```

### Team Deathmatch
```csharp
RoomSettings settings = RoomSettings.CreateTeamDeathmatch(2, 2);
```

### Survival
```csharp
RoomSettings settings = RoomSettings.CreateSurvival(4);
```

## 🐛 Troubleshooting

### Lỗi kết nối
- Kiểm tra AppID trong PhotonServerSettings
- Đảm bảo internet connection
- Kiểm tra firewall settings

### Player không spawn
- Đảm bảo player prefab có PhotonView
- Kiểm tra prefab name trong Resources folder
- Verify MultiplayerManager.playerPrefab assignment

### Enemy không hoạt động
- Đảm bảo enemy prefab có NetworkEnemy component
- Kiểm tra AI settings (detection range, attack range)
- Verify spawn points assignment

## 📚 API Reference

### MultiplayerManager
- `ConnectToServer()` - Kết nối Photon server
- `CreateRoom(name, settings)` - Tạo phòng mới
- `JoinRandomRoom()` - Join phòng ngẫu nhiên
- `SpawnPlayer()` - Spawn player prefab

### NetworkPlayer
- `TakeDamage(damage)` - Nhận damage
- `AddScore(points)` - Thêm điểm
- `IsAlive()` - Kiểm tra player còn sống
- `Respawn()` - Hồi sinh player

### NetworkEnemy
- `TakeDamage(damage)` - Nhận damage
- `IsDead()` - Kiểm tra enemy đã chết
- `SetSpawnPosition(pos)` - Set vị trí spawn

## 🤝 Contributing

1. Fork project
2. Tạo feature branch
3. Commit changes
4. Push to branch
5. Tạo Pull Request

## 📄 License

MIT License - xem file LICENSE để biết thêm chi tiết.

## 🆘 Support

Nếu gặp vấn đề, hãy:
1. Kiểm tra README này
2. Xem console logs
3. Verify Photon settings
4. Tạo issue với thông tin chi tiết 