using UnityEngine;
using UnityEngine.InputSystem; // Namespace pour utiliser le nouveau système d'input
using Unity.Netcode; // Namespace pour utiliser Netcode

/* Script du prefab (la barre) joueur identifié comme étant le joueur (Default player prefab) dans le NetWorkManager.
Il sera automatiquement instancié (spawn) pour chaque client qui se connecte.
*/

// c'est un objet réseau (NetworkObject). Le script doit dériver de NetworkBehaviour
public class Barre : NetworkBehaviour
{
    public float[] limites = new float[2];
    [Tooltip("Vitesse en secondes")]
    public float vitesse;

    InputAction moveAction;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
        //Debug.Log(Time.deltaTime);
    }


    /* OnNetworkSpawn() est une fonction semblabe au Start, mais pour les objets réseaux. Exécuté avant le Start qui pourrait aussi
     être utilisé. Voici l'ordre d'exécution des fonctions d'initialisation :
     1- Awake()
     2- OnNetworkSpawn()
     3- Start()

     Le mot override indique que cette fonction est déjà présente dans un autre script (classe) pour les objets qui héritent
     du NetworkBehavior. On doit donc indiquer que c'est un override et la première ligne de la fonction fait en sorte
     que la base de cette fonction est aussi exécutée.

     On place la barre du joueur : à gauche pour l'hôte et à droite pour l'autre client. */
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); //

        if (IsServer)
        {
            transform.position = new Vector3(-20f, 0.5f, 0f); //position à ajuster selon votre jeu
        }
        else
        {
            transform.position = new Vector3(20f, 0.5f, 0f); //position à ajuster selon votre jeu
        }

        moveAction = InputSystem.actions.FindAction("Move");
    }

    /* Dans le Update, on appelle la fonction qui gère les touches et le déplacement seulement si on est le joueur local  
    Cela permet seulement au joueur local de contrôler les déplacements de sa barre.

    Il ne faut pas oublier que le Update des 2 barres s'exécute sur le client hôte (serveur) et le client qui n'est pas serveur. 
    On ignore donc 2 cas de figure en procédant ainsi : 
    1- La barre du client qui n'est pas serveur sur l'hôte : pas d'appel de fonction;   
    2- La barre du client-hôte (serveur) sur le client qui n'est pas serveur : pas d'appel de fonction
    3- La barre du client-hôte (serveur) sur le client-hôte (serveur) : appel de fonction
    4- La barre du client qui n'est pas serveur sur le client qui n'est pas serveur : appel de fonction */
    void Update()
    {
        if (!IsOwner) return;
        GestionDeplacement(); // Appeler ici votre propre fonction de déplacement
    }

    void GestionDeplacement()
    {
        // votre code…
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        //Debug.Log(moveValue);
        Vector3 newPosValue = new(transform.position.x, transform.position.y,
            Mathf.Clamp(
                transform.position.z + (((moveValue.x + moveValue.y) * vitesse) * Time.deltaTime),
                limites[0], limites[1]
                )
            );
        transform.position = newPosValue;
        //transform.position = new(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, limites[0], limites[1]));
    }

}
