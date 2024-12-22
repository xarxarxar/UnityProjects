using System.Collections;
using UnityEngine;

public class CtrTitle : CtrBase
{
    public LoadingAnim loadingAnim;

    protected override void Start()
    {
        StartCoroutine(StartCo());
        base.Start();
    }
    
    IEnumerator StartCo()
    {
        loadingAnim.SetLoading(true);
        yield return StartCoroutine(LogInCheckCo());
        PlayManager.Instance.LoadScene(Data.scene_home);
    }

    IEnumerator LogInCheckCo()
    {
        bool isLogin = false;
        while (!isLogin)
        {
            yield return new WaitForSeconds(2f);
            isLogin = true;
        }
        
        //로그인 성공
    }
}
