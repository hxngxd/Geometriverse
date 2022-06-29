using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawLine : MonoBehaviour
{
    Draw draw;
    LineCollider linecollder;
    public float ratio;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        linecollder = FindObjectOfType<LineCollider>();
    }

    public IEnumerator Okay(){
        draw.mouse.UnselectAll();
        while (true){
            draw.drawing = true;
            StartCoroutine(draw.point.PointInit(()=>{}, ()=>{}, Cancel));
            yield return new WaitUntil(() => !draw.point.pointing);
            
            var line = draw.obj.Line(draw.hierContent);
            Hierarchy.currentObjects["line"].Add(line.gameObject);
            line.positionCount++;
            line.SetPosition(0, Hierarchy.currentObjects["point"][0].transform.position);

            var start = Hierarchy.currentObjects["point"][0].name;
            var overlapsedStart = draw.point.overlapsed;
            if (overlapsedStart) Hierarchy.currentObjects["point"].RemoveAt(0);

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.PointInit(()=>{
                if (line.positionCount < 2) line.positionCount++;
                line.SetPosition(1, Hierarchy.currentObjects["point"][overlapsedStart ? 0 : 1].transform.position);
                line.startWidth = Vector3.Distance(Camera.main.transform.position, line.GetPosition(0))*ratio;
                line.endWidth = Vector3.Distance(Camera.main.transform.position, line.GetPosition(1))*ratio;
            }, ()=>{}, Cancel));
            yield return new WaitUntil(() => !draw.point.pointing);

            var end = Hierarchy.currentObjects["point"][overlapsedStart ? 0 : 1].name;

            bool overlapseLine = false;
            foreach (var l in Hierarchy.Lines){
                if ((l.Value.start == start && l.Value.end == end) || (l.Value.start == end && l.Value.end == start)){
                    overlapseLine = true;
                    Destroy(line.gameObject);
                    line = l.Value.go;
                    break;
                }
            }

            if (!overlapseLine){
                draw.hier.AddLine("", start, end, "", line, new List<string>());
                linecollder.AddCollider(line);
            }

            draw.hier.ResetCurrentObjects();
            yield return new WaitForSeconds(0.015f);
        }
    }
    public IEnumerator OnSelect(LineRenderer line){
        draw.mouse.Select(line.transform);
        
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        Cancel();
        draw.mouse.Unselect(line.transform);
    }
    public void Cancel(){
        draw.Cancel();
    }
    
}
