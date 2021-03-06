using GameJolt.UI;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    public class UpgradeButton : MonoBehaviour
    {
        public Upgrade upgrade;
        public Control player;
        public UpgradeStore store;
        public TextMeshProUGUI label;
        public TextMeshProUGUI description;
        public TextMeshProUGUI price;
        public Sprite warningSprite;
        public Image iconImage;

        protected int _level;

        public void Start()
        {
            _level = upgrade.startLevel;
            
            RerenderUi();
        }

        public void Buy()
        {
            if (player.GetMoney() < GetPrice())
            {
                GameJoltUI.Instance.QueueNotification("Not enough money", warningSprite);
                return;
            }
            
            // Reduce money
            player.RemoveMoney(GetPrice());

            // Increase level (if it is now >= max, disable button and hide price)
            _level++;

            if (_level >= upgrade.maxLevels)
            {
                GetComponent<Button>().interactable = false;
                price.gameObject.SetActive(false);
            }

            RerenderUi();

            // Run custom buy logic
            Debug.Log("Bought: " + upgrade.name);
            
            store.GetHandler(upgrade)?.Handle(upgrade, _level, store);

            if (upgrade.removeAfterBuy)
            {
                Destroy(gameObject);
            }
        }

        protected int GetPrice()
        {
            return _level == 0 ? upgrade.price : Mathf.FloorToInt(upgrade.price * (_level * upgrade.priceMultiplicator));
        }

        protected virtual void RerenderUi()
        {
            label.text = $"{upgrade.label} <size=16>(Level {_level}/{upgrade.maxLevels})</size>";
            description.text = upgrade.description;
            price.text = $"${GetPrice()}";
            iconImage.sprite = upgrade.icons[_level];
        }
    }
}