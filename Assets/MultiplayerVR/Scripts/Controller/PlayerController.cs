using UnityEngine;
using UnityEngine.Networking;

namespace MultiplayerVR
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        public Transform bulletSpawn;


        public override void OnStartLocalPlayer()
        {
            GetComponent<Renderer>().material.color = Color.blue;

            // attach camera to player.. 3rd person view..
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(0, 1.33f, -0.69f);
            Camera.main.transform.localRotation = Quaternion.Euler(6.31f, 0, 0);
        }

        void Update()
        {

            if (!isLocalPlayer)
            {
                return;
            }

            var x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

            transform.Translate(x, 0, z);

            transform.rotation = Camera.main.transform.rotation;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdFire();
            }
        }

        [Command]
        protected void CmdFire()
        {
            // Create the Bullet from the Bullet Prefab
            var bullet = (GameObject)Instantiate(
                bulletPrefab,
                bulletSpawn.position,
                bulletSpawn.rotation);

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

            // Spawn the bullet on the Clients
            NetworkServer.Spawn(bullet);

            // Destroy the bullet after 2 seconds
            Destroy(bullet, 2.0f);
        }
    }
}