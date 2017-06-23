﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormHeadScript : MonoBehaviour {
    public List<GameObject> Worms;
    public Vector3[] postions;
    public GameObject Worm;
    public GameObject targetObj;
    CanonScript canon;
    public float speed, hp, atackedTime,delay;
    public int length;
    public float updateDelay;
    public bool isDamaged;
    float timer;
    // Use this for initialization
    void Start()
    {
        canon = GameObject.FindObjectOfType<CanonScript>();
        //초기화
        postions = new Vector3[Worms.Count];
        updateDelay = 1 / speed;
        //라인렌더러 초기화

        for (int i = 0; i < Worms.Count; i++)
        {
            postions[i] = transform.position;
        }


        StartCoroutine(UpdateLine());   //updateDelay주기로 업데이트
    }

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        if (!isDamaged)
        {
            atackedTime += Time.deltaTime;
            if (atackedTime > delay)
            {
                isDamaged = true;
                atackedTime = 0;
            }
                
        }
            
        if (hp <= 0)
            Destroy(gameObject);
    }

    IEnumerator UpdateLine()
    {
        while (true)
        {
            postions[0] = transform.position;   //현재 자신의 위치를 0번째 postion에 저장

            //나머지 위치를 한칸 씩 밀어냄
            for (int i = length - 1; i > 0; i--)
            {
                postions[i] = postions[i - 1];
            }
            for (int i = length - 1; i > 0; i--)
            {
                Worms[i].transform.position = postions[i];
            }
            //pos배열을 line renderer에 적용
            yield return new WaitForSeconds(updateDelay);

            //반복
        }

    }
    //IEnumerator ResizeWorms()
    //{
    //    for(int i=length-1;)
    //    }
    //}
    void OnCollisionEnter(Collision col)
    {
        if(col.transform.tag=="Wall")
        {
            DrawLaser(transform.position);
        }
        else if(col.transform.tag=="CanonWall")
        {
            DrawLaser(transform.position);
            for(int i=0;i<length;i++)
            {
                GameObject tmp = Instantiate(Worm);
                tmp.transform.position = transform.position;
                tmp.GetComponent<WormScript>().head = GetComponent<WormHeadScript>();
                Worms.Add(tmp);
            }
            length *= 2;
            initWorms();
            canon.hp -= 10;
            //Instantiate(prefebs);
            //Worms.Add(prefebs);
            
            //length *= 2;

        }
        else if(col.transform.tag=="Worm")
        {
            //Do nothing
        }
    }

    public void damage(float damage)
    {
        if(isDamaged)
        {
            canon.hp += 10;
            hp -= damage;
            isDamaged = false;
        }
    }
    void DrawLaser(Vector3 startPoint)
    {
        
        RaycastHit hit;
        Vector3 rayDir = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(startPoint, rayDir, out hit, Mathf.Infinity))
        {
            //Debug.Log("Infinity" + Mathf.Infinity);
            //Debug.Log("hit" + hit);
            //Debug.Log("startPoint" + startPoint);
            //Debug.Log("rayDir"+rayDir);
            rayDir = Vector3.Reflect((hit.point - startPoint).normalized, hit.normal);
            if (Physics.Raycast(startPoint, rayDir, out hit, Mathf.Infinity))
            {
                GameObject target = Instantiate(targetObj) as GameObject;
                target.transform.position = hit.point;
                transform.LookAt(target.transform);
            }
           
        }
    }
    public void initWorms()
    {
        postions = new Vector3[Worms.Count];
        for (int i = 0; i < Worms.Count; i++)
        {
            postions[i] = Worms[i].transform.position;
        }
    }
}
