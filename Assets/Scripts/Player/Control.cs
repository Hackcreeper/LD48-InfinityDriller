using GameJolt.API;
using GameJolt.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Player
{
    public class Control : MonoBehaviour
    {
        public float speed = 16;
        public LevelGenerator level;
        public int minAngle = -20;
        public int maxAngle = 20;
        public int maxEnergy = 100;
        public RectTransform energyBar;
        public float maxHeightEnergyBar;
        public float energyEfficiency = 1;
        public int score;
        public TextMeshProUGUI scoreLabel;
        public TextMeshProUGUI rankLabel;
        public TextMeshProUGUI moneyLabel;
        public GameObject guestGameOverScreen;
        public RectTransform heatBar;
        public Sprite warnSprite;
        public Volume volume;
        public float heatSpeedModificator = .1f;
        public float cooldown = .1f;

        private float _angle;
        private bool _pressingLeft;
        private bool _pressingRight;
        private float _energy;
        private bool _dead;
        private int _money;
        private float _heat;
        private Vignette _vignette;
        private float _realSpeed;

        private void Awake()
        {
            _energy = maxEnergy;
            _realSpeed = speed;
            volume.profile.TryGet(out _vignette);
        }

        private void Update()
        {
            if (_dead)
            {
                return;
            }
            
            if (!_pressingLeft && !_pressingRight)
            {
                _angle = 0;
            }

            if (_pressingLeft)
            {
                _angle = minAngle;
            }
            
            if (_pressingRight)
            {
                _angle = maxAngle;
            }

            if (_pressingLeft && _pressingRight)
            {
                _angle = 0;
            }

            _realSpeed = speed - (heatSpeedModificator * _heat);
            
            transform.Translate(0, -_realSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.Euler(0, 0, _angle);

            _energy -= energyEfficiency * Time.deltaTime;

            energyBar.sizeDelta = new Vector2(
                energyBar.sizeDelta.x,
                maxHeightEnergyBar / maxEnergy * _energy
            );

            heatBar.sizeDelta = new Vector2(
                heatBar.sizeDelta.x,
                85f / 100 * _heat
            );

            _heat = Mathf.Clamp(_heat - cooldown * Time.deltaTime, 0, 100);
            
            // base is 0.451
            if (_heat >= 70)
            {
                _vignette.intensity.value = 0.451f + 0.549f / 30 * (_heat - 70);
                _vignette.color.value = Color.red;
            }
            else
            {
                _vignette.intensity.value = 0.451f;
                _vignette.color.value = Color.black;   
            }
            
            scoreLabel.text = score.ToString();
            moneyLabel.text = $"${_money.ToString()}";
            
            if (_energy <= 0)
            {
                GameJoltUI.Instance.QueueNotification("You ran out of energy!");
                Die();
            }

            if (_heat >= 100)
            {
                GameJoltUI.Instance.QueueNotification("You overheated!");
                Die();
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                GameJoltUI.Instance.QueueNotification("You killed yourself!", warnSprite);
                Die();
            }
            
            if (Application.isEditor && Input.GetKeyDown(KeyCode.J))
            {
                _heat += 10;
            }
            
            if (Application.isEditor && Input.GetKeyDown(KeyCode.L))
            {
                GameJoltUI.Instance.ShowSignIn();
            }

            if (score <= 0)
            {
                return;
            }
            
            Scores.GetRank(score, 618313, (int rank) =>
            {
                rankLabel.text = $"Rank {rank}";
            });
        }

        private void Die()
        {
            _dead = true;
            
            if (GameJoltAPI.Instance.HasSignedInUser)
            {
                Scores.Add(score, score.ToString(), 618313, null, (bool success) =>
                {
                    GameJoltUI.Instance.ShowLeaderboards(); 
                });
                return;
            }
            
            guestGameOverScreen.SetActive(true);
        }

        public void MoveLeft(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _pressingLeft = false;
                return;
            }

            _pressingLeft = true;
        }

        public void MoveRight(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _pressingRight = false;
                return;
            }

            _pressingRight = true;
        }

        public void RestoreEnergy(float add)
        {
            _energy = Mathf.Clamp(_energy + add, 0, maxEnergy);
        }

        public void AddMoney(int add)
        {
            _money += add;
        }
    }
}