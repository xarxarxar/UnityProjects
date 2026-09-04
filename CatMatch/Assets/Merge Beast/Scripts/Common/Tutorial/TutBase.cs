using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using Observer;
using MergeBeast;
using UnityEngine.SceneManagement;

public class TutBase : MonoBehaviour
{
    [SerializeField] protected GameObject hand;
    // [SerializeField] GameObject wave;
    // [SerializeField] GameObject swipe;
    [SerializeField] protected GameObject focus;
    [SerializeField] protected Button btnAction;
    [SerializeField] protected Button btnNext;
    [SerializeField] protected Text txtDialog;
    // [SerializeField] Animator dialogAnim;

    private UnityAction currentAction;
    private UnityAction nextAction;
    private bool focusShown;

    private Coroutine dialogCoro;
    private string currentText;
    private bool textDone = false;

    protected virtual void Start()
    {
        btnAction.onClick.AddListener(() =>
               {
                   if (currentAction != null)
                   {
                       currentAction.Invoke();
                       //currentAction = null; 

                   }
                   else Debug.Log("nulllllllllllllll");
               });

        btnNext.onClick.AddListener(() => OnClickNextText());
    }

    public void SetSkipAction(UnityAction cb)
    {
        nextAction = cb;
    }

    public void ResetAllEffect()
    {
        StopAllCoroutines();

        hand.SetActive(false);
        // wave.SetActive(false);
        // swipe.SetActive(false);
    }

    public void ShowFocus(Vector2 pos, UnityAction callback)
    {
        currentAction = callback;
        ResetAllEffect();
        StartCoroutine(_ShowFocus(pos));
    }

    IEnumerator _ShowFocus(Vector2 pos)
    {
        yield return new WaitForSeconds(.2f);
        focusShown = true;
        Debug.Log("show focus");
        focus.gameObject.SetActive(true);
        focus.transform.position = pos;

        while (focusShown)
        {
            HandTouch(pos);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void HideFocus()
    {
        focusShown = false;
        focus.gameObject.SetActive(false);
    }

    public void HideHand()
    {
        focusShown = false;
        hand.SetActive(false);
    }

    void HandTouch(Vector2 pos)
    {
        hand.SetActive(true);
        Vector2 posAnim = pos + new Vector2(0.5f, -0.5f);
        hand.transform.position = posAnim;
        hand.transform.DOMove(pos, 0.2f);

    }

    public void ShowOnlyHand(Vector2 pos)
    {
        HandTouch(pos);
    }

    public void ShowText(string msg, UnityAction cb = null)
    {
        nextAction = cb;
        btnNext.gameObject.SetActive(true);
        // if(showAnim)
        // dialogAnim.SetTrigger("Show");
        if (dialogCoro != null)
        {
            StopCoroutine(dialogCoro);
            dialogCoro = null;
        }
        if (!string.IsNullOrEmpty(msg))
            dialogCoro = StartCoroutine(_ShowText(msg));
        else txtDialog.text = "";
    }

    public void HideText()
    {
        btnNext.gameObject.SetActive(false);
        // dialogAnim.SetTrigger("Hide");

    }

    IEnumerator _ShowText(string msg)
    {
        textDone = false;
        currentText = msg;
        txtDialog.text = "";
        yield return new WaitForSeconds(0.5f);
        foreach (char c in msg.ToCharArray())
        {
            txtDialog.text += c;
            yield return new WaitForSeconds(0.01f);
        }
        textDone = true;
    }

    public void OnClickNextText()
    {
        if (textDone)
        {
            //HideText();
            if (nextAction != null)
            {
                nextAction.Invoke();
                //nextAction = null;
            }
        }
        else
        {
            if (dialogCoro != null)
            {
                StopCoroutine(dialogCoro);
                dialogCoro = null;
                txtDialog.text = currentText;
                textDone = true;
            }
        }
    }

}
