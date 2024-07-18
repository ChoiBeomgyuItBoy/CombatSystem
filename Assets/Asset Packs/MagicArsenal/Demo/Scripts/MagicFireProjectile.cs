using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace MagicArsenal
{
    public class MagicFireProjectile : MonoBehaviour
    {
        public GameObject[] projectiles;

        [Header("GUI Links")]
        public Text missileNameText; // Reference to the Text object for displaying the missile name.
		public UnityEngine.UI.Toggle fullAutoButton;
		public Slider speedSlider; // Reference to the Speed Slider UI element

        [Header("Projectile Settings")]
        public Transform spawnPosition;
        [HideInInspector]
        public int currentProjectile = 0;
        public float speed = 1000;
        public float spawnOffset = 0.3f; // Customizable setting for offset

        [Header("Firing Settings")]
        public float fireRate = 0.13f;
        public bool isFullAuto = true;


        [Header("Gun Settings")]
        public GameObject gunPrefab; // Reference to the gun prefab
        public float gunOffset = 0.5f; // Customizable setting for the gun's offset from the player

        private bool canShoot = true;
        private GameObject instantiatedGun; // Reference to the instantiated gun

        private void Start()
        {
            if (gunPrefab != null)
            {
                instantiatedGun = Instantiate(gunPrefab, Vector3.zero, Quaternion.identity);
                instantiatedGun.transform.SetParent(transform); // Set the gun's parent to the player
                instantiatedGun.transform.localPosition = Vector3.zero; // Reset the local position relative to the player
            }
			
			if (speedSlider != null)
            {
                speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);
                // Set the initial speed value based on the slider's starting value
                speed = speedSlider.value;
            }
			
			if (fullAutoButton != null)
            {
				fullAutoButton = GameObject.Find("ToggleAuto").GetComponent<Toggle>();
			}
			
            UpdateDisplayName();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                nextEffect();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                previousEffect();
            }

            if (fullAutoButton != null)
            {
                isFullAuto = fullAutoButton.isOn;
            }
			
			if (instantiatedGun != null) // Add this null check
			{
				UpdateGunPositionAndRotation();
			}

            if (isFullAuto)
            {
                if (canShoot && Input.GetKey(KeyCode.Mouse0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        StartCoroutine(Shoot());
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        ShootProjectile();
                    }
                }
            }
			if (speedSlider != null)
			{
				speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);
				// Set the initial speed value based on the slider's starting value
				speed = speedSlider.value;
			}

            Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 100, Color.yellow);
        }

        private IEnumerator Shoot()
        {
            canShoot = false;
            ShootProjectile();
            yield return new WaitForSeconds(fireRate);
            canShoot = true;
        }

        private void ShootProjectile()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 direction;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                direction = (hit.point - spawnPosition.position).normalized;
            }
            else
            {
                direction = ray.direction.normalized;
            }

            Vector3 spawnPositionWithOffset = spawnPosition.position + direction * spawnOffset;
            GameObject projectile = Instantiate(projectiles[currentProjectile], spawnPositionWithOffset, Quaternion.identity);
            projectile.transform.LookAt(spawnPositionWithOffset + direction * 10f);
            projectile.GetComponent<Rigidbody>().AddForce(direction * speed);
        }

        private void UpdateGunPositionAndRotation()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			Vector3 targetPoint;

			if (Physics.Raycast(ray, out hit))
			{
				targetPoint = hit.point;
			}
			else
			{
				targetPoint = ray.origin + ray.direction * 100f;
			}

			Vector3 directionToMouse = (targetPoint - transform.position).normalized;
			float horizontalAngle = Mathf.Atan2(directionToMouse.x, directionToMouse.z) * Mathf.Rad2Deg;
			float verticalAngle = -Mathf.Asin(directionToMouse.y / directionToMouse.magnitude) * Mathf.Rad2Deg;

			Quaternion targetRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

			if (instantiatedGun != null)
			{
				instantiatedGun.transform.rotation = Quaternion.Slerp(instantiatedGun.transform.rotation, targetRotation, Time.deltaTime * 10f);
				instantiatedGun.transform.position = spawnPosition.position - instantiatedGun.transform.forward * gunOffset;
			}
		}

        public void nextEffect()
        {
            if (currentProjectile < projectiles.Length - 1)
                currentProjectile++;
            else
                currentProjectile = 0;

            UpdateDisplayName();
        }

        public void previousEffect()
        {
            if (currentProjectile > 0)
                currentProjectile--;
            else
                currentProjectile = projectiles.Length - 1;

            UpdateDisplayName();
        }

        private void UpdateDisplayName()
        {
            Text displayText = missileNameText != null ? missileNameText : GetComponentInChildren<Text>();
            if (displayText != null)
            {
                MagicProjectileScript projectileScript = projectiles[currentProjectile].GetComponent<MagicProjectileScript>();
                string projectileParticleName = projectileScript.projectileParticle.name;
                displayText.text = projectileParticleName;
            }
        }
		
		private void OnSpeedSliderChanged(float value)
        {
            // Update the speed based on the slider value
            speed = value;
        }
    }
}