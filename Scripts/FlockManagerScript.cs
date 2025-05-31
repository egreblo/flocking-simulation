using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FlockManagerScript : MonoBehaviour
{
    public Boid boidPrefab;
    List<Boid> boids;
    public int n = 10;
    float maxDistance = 4.75f;
    bool play = false;
    public bool[] rules = {true,true,true,true};
    public bool FREEZE_TIME = false;
    Vector3 mousePoint;
    Vector3 mouseReflectPoint;
    public float maxSpeed = 1;
    float[] boundries = {0,0,0,0};
    public float animationSpeed = 0.1f;
    void Start()
    {
        boids = new List<Boid>();
        mousePoint = new Vector3(0,0,0);
        mouseReflectPoint = new Vector3(0,0,0);
        CalculateBoundries();
        InitialiseBoids();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("n"))
            StartCoroutine(MoveBoids());
        if(Input.GetKeyDown("r")){
            play = false; 
            foreach(Boid b in boids)
                Destroy(b.gameObject);
            Start();
        }
            
        if(Input.GetKeyDown("s"))
            play = !play; 
        if(Input.GetMouseButton(0)){
            mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint = new Vector3(mousePoint.x, mousePoint.y, 0f);
        }else{
            mousePoint = new Vector3(0,0,0);
        }
         if(Input.GetMouseButton(1)){
            mouseReflectPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseReflectPoint = new Vector3(mouseReflectPoint.x, mouseReflectPoint.y, 0f);
        }else{
            mouseReflectPoint = new Vector3(0,0,0);
        }

        StartCoroutine(MoveBoids());
    }

    private IEnumerator MoveBoids()
    {
        foreach (Boid b in boids){

            List<Boid> neighbours = GetNeighbours(b);
            if(play){ 
                if(neighbours.Count > 0){
                    Vector3 separation = Separation(neighbours, b);
                    Vector3 alignment = Alignment(neighbours, b);
                    Vector3 cohesion = Cehosion(neighbours, b);
                    if(!rules[0]) separation = new Vector3(0f,0f,0f);
                    if(!rules[1]) alignment = new Vector3(0f,0f,0f);
                    if(!rules[2]) cohesion = new Vector3(0f,0f,0f);

                    b.velocity += separation + alignment + cohesion;
                }
                if(rules[3]){
                    Vector3 tendToPlace = TendToPlace(mousePoint, b);
                    Vector3 tendAwayPlace = TendAwayPlace(mouseReflectPoint, b);
                    Vector3 boundPosition = BoundPosition(b);
                    b.velocity += tendToPlace + boundPosition + tendAwayPlace;
                    LimitVelocity(b);
                }
                b.transform.position = b.transform.position + b.velocity;
            }
        }
        yield return new WaitForSeconds(animationSpeed);


    }

    private Vector3 BoundPosition(Boid b)
    {
        Vector3 bound = new Vector3(0,0,0);
        float reppelSpeed = b.velocity.magnitude / 5;
        if(b.transform.position.x < boundries[0]){
            bound.x = reppelSpeed;
        }else if(b.transform.position.x > boundries[1]){
            bound.x = -reppelSpeed;
        }
        if(b.transform.position.y < boundries[2]){
            bound.y = reppelSpeed;
        }else if(b.transform.position.y > boundries[3]){
            bound.y = -reppelSpeed;
        }
        return bound;
    }

    private void LimitVelocity(Boid b)
    {
        if(b.velocity.magnitude > maxSpeed){    
            b.velocity = b.velocity / b.velocity.magnitude * maxSpeed;
        }
    }

    private Vector3 TendAwayPlace(Vector3 mouseAwayPoint, Boid b)
    {
        if(mouseAwayPoint.magnitude == 0) return new Vector3(0,0,0);
        return -(mouseAwayPoint - b.transform.position) / 10;
    }

    private Vector3 TendToPlace(Vector3 mousePoint, Boid b)
    {
        if(mousePoint.magnitude == 0) return new Vector3(0,0,0);
        return (mousePoint - b.transform.position) / 100;
    }

    private Vector3 Cehosion(List<Boid> neighbours, Boid boid)
    {
        Vector3 cohesion = new Vector3(0f,0f,0f);

        foreach(Boid b in neighbours){
            cohesion += b.transform.position;
        }
        cohesion /= neighbours.Count;

        return  (cohesion - boid.transform.position) / 100;
    }

    private Vector3 Alignment(List<Boid> neighbours, Boid boid)
    {
        Vector3 avgAlignment = new Vector3(0f,0f,0f);

        foreach(Boid b in neighbours){
            avgAlignment += b.velocity.normalized;
        }
        avgAlignment /= neighbours.Count;

        return avgAlignment / 8;
    }

    private Vector3 Separation(List<Boid> neighbours, Boid boid)
    {
        Vector3 separation = new Vector3(0f,0f,0f);

        foreach(Boid b in neighbours){
            if((b.transform.position - boid.transform.position).magnitude < maxDistance/2){
                separation -= b.transform.position - boid.transform.position;
            }
        }

        return separation / 100;
    }
 
    List<Boid> GetNeighbours(Boid boid){

        List<Boid> neighbours = new List<Boid>();

        foreach(Boid b in boids){
            if( (b.transform.position - boid.transform.position).magnitude < maxDistance && b != boid){
                neighbours.Add(b);
            }
        }

        if(boid.isMain){
            Transform radiusTransform = boid.transform.Find("Radius");
            SpriteRenderer circleSpriteRenderer = radiusTransform.GetComponent<SpriteRenderer>();
            circleSpriteRenderer.color = new Color(1f, 1f, 1f, 0.15f);
            Transform body = boid.transform.Find("Body");
            SpriteRenderer bodySR = body.GetComponent<SpriteRenderer>();
            bodySR.color = new Color(1f, 0.5f, 0f, 1f);

            Transform sqMain = boid.transform.Find("Square");
            SpriteRenderer sqMainSR = sqMain.GetComponent<SpriteRenderer>();
            sqMainSR.color = new Color(202f/255f, 255f/255f, 129f/255f, 1f);
            

            foreach(Boid n in neighbours){
                Transform radiusTransforms = n.transform.Find("Body");
                SpriteRenderer circleSpriteRendererr = radiusTransforms.GetComponent<SpriteRenderer>();
                circleSpriteRendererr.color = new Color(1f, 0.1f, 0.1f, 1f);

                
                Transform sq = n.transform.Find("Square");
                SpriteRenderer sqSR = sq.GetComponent<SpriteRenderer>();
                sqSR.color = new Color(202f/255f, 255f/255f, 129f/255f, 1f);
            }

            foreach(Boid b in boids){
                if(!neighbours.Contains(b) && b!= boid){
                    Transform radius = b.transform.Find("Body");
                    SpriteRenderer circleSpriteRendererr = radius.GetComponent<SpriteRenderer>();
                    circleSpriteRendererr.color = new Color(195f/255f, 195f/255f, 195f/255f, 1f);

                    Transform sq = b.transform.Find("Square");
                    SpriteRenderer sqSR = sq.GetComponent<SpriteRenderer>();
                    sqSR.color = new Color(202f/255f, 255f/255f, 129f/255f, 0f);
                }
            }
                    
        }

        return neighbours;
    }

    void CalculateBoundries(){

        //bottom left
        Vector3 bottomLeft = new Vector3(0,0,0);
        Vector3 bottomLeftPoint = Camera.main.ViewportToWorldPoint(bottomLeft);
        Vector3 topRight = new Vector3(1, 1, 0);
        Vector3 topRightPoint = Camera.main.ViewportToWorldPoint(topRight);

        boundries[0] = bottomLeftPoint.x;
        boundries[1] = topRightPoint.x;
        boundries[2] = bottomLeftPoint.y;
        boundries[3] = topRightPoint.y;
      }

    void InitialiseBoids(){

        for(int i = 0; i < n; i++){

            Vector2 position = new Vector3(UnityEngine.Random.Range(boundries[0], boundries[1]), 
                                           UnityEngine.Random.Range(boundries[2], boundries[3]), 0f);
            Boid boid = Instantiate(boidPrefab, position, Quaternion.identity);
            boid.transform.parent = transform;
            boids.Add(boid);
        }
    }
}
