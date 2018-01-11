using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorCam : MonoBehaviour
{
    public Vector3 StartPos;
    public Vector3 EndPos;
    public float time;
    private Vector3 deltaPos;
    private float elapsedTime;
    private bool bStartToEnd = true;
    void Start()
    {
        // StartPosをオブジェクトに初期位置に設定
        transform.position = StartPos;
        // 1秒当たりの移動量を算出
        deltaPos = (EndPos - StartPos) / time;
        elapsedTime = 0;
    }

    void Update()
    {
        // 1秒当たりの移動量にTime.deltaTimeを掛けると1フレーム当たりの移動量となる
        // Time.deltaTimeは前回Updateが呼ばれてからの経過時間
        transform.position += deltaPos * Time.deltaTime;
        // 往路復路反転用経過時間
        elapsedTime += Time.deltaTime;
        // 移動開始してからの経過時間がtimeを超えると往路復路反転
        if (elapsedTime > time)
        {
            if (bStartToEnd)
            {
                // StartPos→EndPosだったので反転してEndPos→StartPosにする
                // 現在の位置がEndPosなので StartPos - EndPosでEndPos→StartPosの移動量になる
                deltaPos = (StartPos - EndPos) / time;
                // 誤差があるとずれる可能性があるので念のためオブジェクトの位置をEndPosに設定
                transform.position = EndPos;
            }
            else
            {
                // EndPos→StartPosだったので反転してにStartPos→EndPosする
                // 現在の位置がStartPosなので EndPos - StartPosでStartPos→EndPosの移動量になる
                deltaPos = (EndPos - StartPos) / time;
                // 誤差があるとずれる可能性があるので念のためオブジェクトの位置をSrartPosに設定
                transform.position = StartPos;
            }
            // 往路復路のフラグ反転
            bStartToEnd = !bStartToEnd;
            // 往路復路反転用経過時間クリア
            elapsedTime = 0;
        }
    }
}
//    Vector3 start_posi;
//    Vector3 move_posi;

//    void Start()
//    {
//        start_posi = transform.position; //スタート地点を記憶
//        //this.transform.localPosition = new Vector3(0, 41.2f, -2.0f);
//    }

//    void Update()
//    {
//        //this.transform.localPosition = new Vector3(0, transform.localPosition.y, -2.0f);
//        //move_posi = new Vector3(0, Mathf.Sin(Time.time), 0); //移動する値を取得
//        move_posi = new Vector3(0, Mathf.PerlinNoise(Time.time, 0) * 0.1f + 0.1f, 0);
//        this.transform.localPosition = start_posi + move_posi; //目的地に移動
//    }
//}
