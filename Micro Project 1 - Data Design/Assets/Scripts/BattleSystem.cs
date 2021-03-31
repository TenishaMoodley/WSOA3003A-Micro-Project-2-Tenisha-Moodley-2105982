using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;

    public Transform PlayerBattleStation;
    public Transform EnemyBattleStation;

    Unit PlayerUnit;
    Unit EnemyUnit;

    public TMP_Text DialogueText;

    public BattleHUD PlayerHUD;
    public BattleHUD EnemyHUD;

    public GameObject buttons;

    public GameObject MainUIStuff;
    public GameObject EndScene;

    public Animator EnemyAnim;
    public Animator PlayerAnim;
    public Animator CamAnim;

    public GameObject HealEffect;
    public GameObject LowerEffect;
    public GameObject RiseEffect;

    public GameObject EnemyEffectSpawnPoint;
    public GameObject PlayerEffectSpawnPoint;
    public GameObject EnemyEffectSpawnPointLower;
    public GameObject PlayerEffectSpawnPointLower;

    private void Start()
    {
        EndScene.SetActive(false);
        MainUIStuff.SetActive(true);
        state = BattleState.START;
        buttons.SetActive(false);
        SetupBattle();
        StartCoroutine(SetupBattle());

        EnemyAnim.SetBool("EnemyHit", false);
        EnemyAnim.SetBool("EnemySmoked", false);
        EnemyAnim.SetBool("EnemySplits", false);
        EnemyAnim.SetBool("EnemyHeals", false);

        PlayerAnim.SetBool("PlayerHit", false);
        PlayerAnim.SetBool("PlayerSmoked", false);
        PlayerAnim.SetBool("PlayerSplits", false);
        PlayerAnim.SetBool("PlayerHeals", false);

       
    }

    IEnumerator SetupBattle()
    {
        GameObject PlayerGo = Instantiate(PlayerPrefab, PlayerBattleStation);
        PlayerUnit = PlayerGo.GetComponent<Unit>();

        GameObject EnemeyGo = Instantiate(EnemyPrefab, EnemyBattleStation);
        EnemyUnit = EnemeyGo.GetComponent<Unit>();

        DialogueText.text = "An " + EnemyUnit.UnitName + " Has Appeared!";

        PlayerHUD.SetHUD(PlayerUnit);
        EnemyHUD.SetHUD(EnemyUnit);
        

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;

        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerAttack()
    {
        buttons.SetActive(false);

        if (EnemyUnit.CurrentSpeed >= PlayerUnit.CurrentAcc)
        {
            yield return new WaitForSeconds(2f);
            bool isDead = EnemyUnit.TakeDamage(0);

            EnemyHUD.SetHP(EnemyUnit.CurrentHP);

            DialogueText.text = "Oh No! Enemy Has Dodged!";

            yield return new WaitForSeconds(3f);

            if (isDead) //the enemy
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if(EnemyUnit.CurrentSpeed <= PlayerUnit.CurrentAcc)
        {
            EnemyAnim.SetBool("EnemyHit", true);

            bool isDead = EnemyUnit.TakeDamage(PlayerUnit.Damage);

            EnemyHUD.SetHP(EnemyUnit.CurrentHP);

            DialogueText.text = "Enemy Has Been Hit!";

            yield return new WaitForSeconds(3f);

            if (isDead) //the enemy
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }

    }

    IEnumerator PlayerHeal()
    {
        buttons.SetActive(false);

        if(PlayerUnit.CurrentHP < PlayerUnit.MaxHP)
        {
            DialogueText.text = "You Have Recovered Energy!";
            PlayerAnim.SetBool("PlayerHeals", true);
            Instantiate(HealEffect,PlayerEffectSpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            DialogueText.text = "You Are Already Fully Healed.";
        }

        yield return new WaitForSeconds(4f);

        PlayerUnit.Heal(45);
        PlayerHUD.SetHP(PlayerUnit.CurrentHP);

        yield return new WaitForSeconds(3f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    
     IEnumerator PlayerSmoke() 
    {
        EnemyAnim.SetBool("EnemySmoked", true);

        buttons.SetActive(false);

        EnemyUnit.Smoke(8);
        //PlayerHUD.StatsText.text = "Atk Damage: " + PlayerUnit.Damage + "\n" + "Def: " + PlayerUnit.CurrentDef + "\n" + "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;
        EnemyHUD.StatsText.text = "Atk Damage: " + EnemyUnit.Damage + "\n" + "Def: " + EnemyUnit.CurrentDef + "\n" + "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;

        if (EnemyUnit.CurrentSpeed >= EnemyUnit.MaxSpeed)
        {
            DialogueText.text = "Enemy's SPEED cannot rise anymore!"; //\n Enemy DEF. increased!";

        }
        else if(EnemyUnit.CurrentSpeed <= 0)
        {
            DialogueText.text = "Enemy SPEED cannot lower anymore! \n \n DEF. has increased!"; //\n Enemy DEF. increased!";
            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);
        }
        else if(EnemyUnit.CurrentDef >= EnemyUnit.MaxDef)
        {
            DialogueText.text = "Enemy's SPEED has lowered \n \n Enemy DEF. cannot rise anymore!"; //\n Enemy SPEED lowered!";
            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);
        }
        else if (EnemyUnit.CurrentDef <= 0)
        {
            DialogueText.text = "Enemy DEF. cannot lower anymore!";// \n Enemy SPEED risen!";
            
        }
        else
        {
            DialogueText.text = "Enemy's SPEED has lowered \n \n DEF. has increased!";
            yield return new WaitForSeconds(4f);

            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);
            
            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);

        }

        yield return new WaitForSeconds(4f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
      }

    IEnumerator PlayerDoppel()
    {
        PlayerAnim.SetBool("PlayerSplits", true);

        buttons.SetActive(false);

        EnemyUnit.Doppel(10);
        //PlayerHUD.StatsText.text = "Atk Damage: " + PlayerUnit.Damage + "\n" + "Def: " + PlayerUnit.CurrentDef + "\n" + "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;
        EnemyHUD.StatsText.text = "Atk Damage: " + EnemyUnit.Damage + "\n" + "Def: " + EnemyUnit.CurrentDef + "\n" + "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;
        
        if (EnemyUnit.Damage >= EnemyUnit.MaxDamage)
        {
            DialogueText.text = "Enemy's ACCURACY has lowered. \n \n Enemy ATTACK DAMAGE cannot rise anymore.";// + "\n" + "Enemy ACCURACY lowered!";
            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);

        }
        else if (EnemyUnit.Damage <= 0)
        {
            DialogueText.text = "Enemy ATTACK DAMAGE cannot lower anymore.";// + "\n" + "Enemy ACCURACY lowered!";
        }
        else if (EnemyUnit.CurrentAcc >= EnemyUnit.MaxAcc)
        {
            DialogueText.text = "Enemy ACCURACY cannot rise anymore.\n \n ATTACK DAMAGE has increased!";// + "\n" + "Enemy ATTACK DAMAGE increased!";
            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);

        }
        else if (EnemyUnit.CurrentAcc <= 0)
        {
            DialogueText.text = "Enemy ACCURACY cannot lower anymore.";// + "\n" + "Enemy ATTACK DAMAGE increased!";
        }
        else
        {
            DialogueText.text = "Enemy's ACCURACY has lowered \n \n ATTACK DAMAGE has increased!";
            yield return new WaitForSeconds(4f);

            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);

            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(4f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }



    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            MainUIStuff.SetActive(false);
            EndScene.SetActive(true);
            DialogueText.text = EnemyUnit.UnitName + " Has Been Defeated!";
            
        }
        else if(state == BattleState.LOST)
        {
            MainUIStuff.SetActive(false);
            EndScene.SetActive(true);
            DialogueText.text = "You Lost";
            
        }
        
    }

    IEnumerator PlayerTurn()
    {
        PlayerAnim.SetBool("PlayerHit", false);
        PlayerAnim.SetBool("PlayerHeals", false);
        PlayerAnim.SetBool("PlayerSplits", false);
        PlayerAnim.SetBool("PlayerSmoked", false);

        EnemyAnim.SetBool("EnemyHit", false);
        EnemyAnim.SetBool("EnemySmoked", false);
        EnemyAnim.SetBool("EnemyHeals", false);
        EnemyAnim.SetBool("EnemySplits", false);

        buttons.SetActive(true);

        DialogueText.text = "What Move Will You Choose?";

        yield return new WaitForSeconds(0f);
    }

    IEnumerator EnemyTurn()
    {
        EnemyAnim.SetBool("EnemyHit", false);
        EnemyAnim.SetBool("EnemySmoked", false);
        EnemyAnim.SetBool("EnemyHeals", false);
        EnemyAnim.SetBool("EnemySplits", false);

        PlayerAnim.SetBool("PlayerHit", false);
        PlayerAnim.SetBool("PlayerHeals", false);
        PlayerAnim.SetBool("PlayerSplits", false);
        PlayerAnim.SetBool("PlayerSmoked", false);

        buttons.SetActive(false);
       
        if (PlayerUnit.CurrentSpeed > EnemyUnit.CurrentAcc)
        {
            DialogueText.text = EnemyUnit.UnitName + " ATTACKS!";
            yield return new WaitForSeconds(3f);

            bool isDead = PlayerUnit.TakeDamage(0);

            PlayerHUD.SetHP(PlayerUnit.CurrentHP);

            DialogueText.text = "You Dodged!";

            yield return new WaitForSeconds(3f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }
        else if(EnemyUnit.CurrentHP <= 35) //if enemy's hp below half then heal
        {
            EnemyAnim.SetBool("EnemyHeals", true);

            DialogueText.text = EnemyUnit.UnitName + " HEALS itself!";
            yield return new WaitForSeconds(1f);

            if (EnemyUnit.CurrentHP < EnemyUnit.MaxHP)
            {
                DialogueText.text = "Enemy Recovered Energy!";
                Instantiate(HealEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                DialogueText.text = "Enemy is Fully Healed.";
            }
            yield return new WaitForSeconds(4f);

            EnemyUnit.Heal(35);
            EnemyHUD.SetHP(EnemyUnit.CurrentHP);

            yield return new WaitForSeconds(3f);

            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else if (PlayerUnit.CurrentSpeed > EnemyUnit.CurrentSpeed)
        {
            PlayerAnim.SetBool("PlayerSmoked", true);

            DialogueText.text = EnemyUnit.UnitName + " Releases a dark, ominous SMOKE!";
            yield return new WaitForSeconds(2f);

            PlayerUnit.Smoke(8);
            PlayerHUD.StatsText.text = "Atk Damage: " + PlayerUnit.Damage + "\n" + "Def: " + PlayerUnit.CurrentDef + "\n" + "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;
            //EnemyHUD.StatsText.text = "Atk Damage: " + EnemyUnit.Damage + "\n" + "Def: " + EnemyUnit.CurrentDef + "\n" + "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;

            if (PlayerUnit.CurrentSpeed >= PlayerUnit.MaxSpeed)
            {
                DialogueText.text = "Your SPEED cannot rise anymore!"; 

            }
            else if (PlayerUnit.CurrentSpeed <= 0)
            {
                DialogueText.text = "Your SPEED cannot lower anymore! \n \n DEF. has increased!";
                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);

            }
            else if (PlayerUnit.CurrentDef >= PlayerUnit.MaxDef)
            {
                DialogueText.text = "Your SPEED has lowered \n \n Your DEF. cannot rise anymore!";
                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

            }
            else if (PlayerUnit.CurrentDef <= 0)
            {
                DialogueText.text = "Your DEF. cannot lower anymore!";
            }
            else
            {
                DialogueText.text = "Your SPEED has lowered \n \n DEF. has increased!";
                yield return new WaitForSeconds(2f);

                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(4f);

            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else if (PlayerUnit.CurrentAcc > EnemyUnit.CurrentSpeed)
        {
            EnemyAnim.SetBool("EnemySplits", true);

            DialogueText.text = EnemyUnit.UnitName + " uses DOPPELGANGER and duplicates itself!";
            yield return new WaitForSeconds(2f);

            PlayerUnit.Doppel(10);
            PlayerHUD.StatsText.text = "Atk Damage: " + PlayerUnit.Damage + "\n" + "Def: " + PlayerUnit.CurrentDef + "\n" + "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;
            //EnemyHUD.StatsText.text = "Atk Damage: " + EnemyUnit.Damage + "\n" + "Def: " + EnemyUnit.CurrentDef + "\n" + "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;

            if (PlayerUnit.Damage >= PlayerUnit.MaxDamage)
            {
                DialogueText.text = "Your ACCURACY has lowered \n \n Your ATTACK DAMAGE cannot rise anymore.";
                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

            }
            else if (PlayerUnit.Damage <= 0)
            {
                DialogueText.text = "Your ATTACK DAMAGE cannot lower anymore.";
            }
            else if (PlayerUnit.CurrentAcc >= PlayerUnit.MaxAcc)
            {
                DialogueText.text = "Your ACCURACY cannot rise anymore.";
            }
            else if (PlayerUnit.CurrentAcc <= 0)
            {
                DialogueText.text = "Your ACCURACY cannot lower anymore.\n \n ATTACK DAMAGE has increased!";
                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);

            }
            else
            {
                DialogueText.text = "Your ACCURACY has lowered \n \n ATTACK DAMAGE has increased!";
                yield return new WaitForSeconds(2f);

                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(4f);

            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            PlayerAnim.SetBool("PlayerHit", true);
           

            DialogueText.text = EnemyUnit.UnitName + " ATTACKS!";
            yield return new WaitForSeconds(2f);

            bool isDead = PlayerUnit.TakeDamage(EnemyUnit.Damage);

            PlayerHUD.SetHP(PlayerUnit.CurrentHP);

            DialogueText.text = "You've Been Hit!";

            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }

    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerHeal());
    }

    public void OnSmokeButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerSmoke());
    }

    public void OnDoppelGangButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerDoppel());
    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
