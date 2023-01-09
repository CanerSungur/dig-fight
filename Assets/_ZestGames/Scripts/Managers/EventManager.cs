using System;
using DigFight;
using UnityEngine;

namespace ZestGames
{
    public static class EventManager { }

    public static class GameEvents 
    {
        public static Action OnGameStart, OnLevelSuccess, OnLevelFail, OnChangePhase;
        public static Action<Enums.GameEnd> OnGameEnd, OnChangeScene;
    }

    public static class PlayerEvents 
    {
        public static Action OnMove, OnIdle, OnDie, OnWin, OnLose, OnCheer, OnFly, OnFall, OnLand, OnRevive;
        public static Action OnSetCurrentMovementSpeed, OnSetCurrentMoneyValue, OnSetCurrentPickaxeSpeed, OnSetCurrentPickaxeDurability, OnSetCurrentPickaxePower;
        public static Action OnOpenedUpgradeCanvas, OnClosedUpgradeCanvas;
        public static Action OnEmptyNextInQueue, OnStopSpendingMoney;
        public static Action OnRotateLeft, OnRotateRight, OnResetRotation;
        public static Action OnStartDigging, OnStopDigging, OnStagger, OnStartPushing, OnStopPushing;
        public static Action<PowerUp> OnActivatePickaxeDurability, OnActivatePickaxePower, OnActivatePickaxeSpeed;
    }

    public static class PlayerUpgradeEvents
    {
        public static Action OnOpenCanvas, OnCloseCanvas, OnUpdateUpgradeTexts;
        public static Action<bool> OnUpgradeMovementSpeed, OnUpgradeMoneyValue, OnUpgradePickaxeSpeed, OnUpgradePickaxeDurability, OnUpgradePickaxePower;
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateLevelText, OnUpdateCoinText;
        public static Action<float> OnUpdateCollectableText;
        public static Action<string, FeedBackUi.Colors> OnGiveFeedBack;
    }

    public static class CollectableEvents
    {
        public static Action<float> OnCollect, OnSpend;
        public static Action<int, Vector3> OnSpawnMoney;
    }

    public static class CoinEvents
    {
        public static Action<int> OnCollect, OnSpend;
        public static Action<int, Vector3> OnSpawnCoin;
    }

    public static class InputEvents
    {
        public static Action OnTapHappened, OnTouchStarted, OnTouchStopped;
    }

    public static class PlayerAudioEvents
    {
        public static Action OnPlayCollectMoney, OnPlaySpendMoney, OnPlayCollectCoin, OnPlaySwing, OnEnableJetpackSound, OnDisableJetpackSound, OnStopJetpackSound;
    }

    public static class AiAudioEvents
    {
        public static Action OnPlaySwing, OnEnableJetpackSound, OnDisableJetpackSound, OnStopJetpackSound;
    }

    public static class HapticEvents
    {
        public static Action OnPlayHitBox, OnPlayBreakBox, OnPlayHitExplosive, OnPlayPush;
    }

    public static class PowerUpEvents
    {
        public static Action<ChestBase, PowerUp> OnPickaxeDurabilityTaken, OnPickaxePowerTaken, OnPickaxeSpeedTaken;
    }

    public static class AiEvents
    {
        public static Action OnMove, OnIdle, OnDie, OnWin, OnLose, OnFly, OnFall, OnLand;
        public static Action OnSetCurrentPickaxeSpeed, OnSetCurrentPickaxeDurability, OnSetCurrentPickaxePower;
        public static Action OnStartDigging, OnStopDigging, OnStagger, OnStartPushing, OnStopPushing;
        public static Action<PowerUp> OnActivatePickaxeDurability, OnActivatePickaxePower, OnActivatePickaxeSpeed;
    }
}
