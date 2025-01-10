using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ADanceOfLifeAndDeath : MonoBehaviour
{
    public int vie;
    public int vieMAX;
    private Vector3 positionSpawn;
    private Hearts barreDeVie;
    private DepletedHearts barreDeVide;
    private bool invincible;
    private SpriteRenderer skin;
    public bool tutoRegen;
    private int deathCount = 0;
    public GameObject pauseHud;
    public GameObject hud;

    void Start()
    {
        positionSpawn = transform.position;
        vieMAX = vie;
        barreDeVie = FindObjectOfType<Hearts>();
        barreDeVie.VieUpdate(vie);
        barreDeVide = FindObjectOfType<DepletedHearts>();
        barreDeVide.FairePlace(vieMAX);
        skin = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (tutoRegen)
        {
            StartCoroutine(TutoRegen());
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseHud.GetComponent<PauseMenu>().Pause();
        }
    }

    void TakeDamage(int degats) //registers the damage taken by the player, manages the hitstun, and calls the function that registers death
    {
        if (invincible)
        {
            return;
        }
        vie -= degats;
        invincible = true;
        barreDeVie.VieUpdate(vie);
        if (vie <= 0)
        {
            JeSuisMouru();
        }
        else
        {
            StartCoroutine(GÈMal());
            StartCoroutine(Clignotant());
        }
    }

    public void AddHealth() //Adds health or heals the player, if they buy such things in the game's shop
    {
        if (hud.GetComponent<MoneyCount>().money >= 5)
        {
            if (vieMAX < 6)
            {
                vieMAX++; vie++;
                hud.SendMessage("MoneyDown", 5);
            }
            else if (vie < vieMAX)
            {
                vie++;
                hud.SendMessage("MoneyDown", 5);
            }
            barreDeVide.FairePlace(vieMAX);
            barreDeVie.VieUpdate(vie);
        }
    }

    void OnTriggerEnter2D(Collider2D truc) //Calls the damage function, the death function, and registers the last checkpoint position
    {
        if (truc.CompareTag("Kill"))
        {
            JeSuisMouru();
        }

        if (truc.CompareTag("Damage") || truc.CompareTag("EnemyProjo"))
        {
            TakeDamage(1);
        }

        if (truc.CompareTag("TchËque poing"))
        {
            positionSpawn = transform.position;
            positionSpawn.x += 1f;
        }
    }

    void JeSuisMouru() //Registers the player's death, and loads the game over screen if needed
    {
        invincible = false;
        transform.position = positionSpawn;
        vie = vieMAX;
        barreDeVie.VieUpdate(vie);
        hud.SendMessage("LivesDown", 1);

        deathCount++;
        if(deathCount > 3)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    IEnumerator GÈMal() //Adds a few invincibility frames to the player after a hit
    {
        yield return new WaitForSeconds(0.5f);
        invincible = false;
    }

    IEnumerator Clignotant() //Makes the player flash upon taking damage
    {
        skin.color = new Color(1,1,1,0.2f);
        yield return new WaitForSeconds(0.1f);
        skin.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        skin.color = new Color(1, 1, 1, 0.2f);
        yield return new WaitForSeconds(0.1f);
        skin.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        skin.color = new Color(1, 1, 1, 0.2f);
        yield return new WaitForSeconds(0.1f);
        skin.color = Color.white;
    }

    IEnumerator TutoRegen() //In the tutorial, makes the player regenerate health quickly
    {
        yield return new WaitForSeconds(0.5f);
        if(vie<vieMAX)
        {
            vie++;
            barreDeVie.VieUpdate(vie);
        }
    }
}
