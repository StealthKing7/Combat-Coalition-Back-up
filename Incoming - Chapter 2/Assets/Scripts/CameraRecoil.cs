using UnityEngine;

public class CameraRecoil : MonoBehaviour
{

    private Vector3 CurrentRotation;
    private Vector3 TargetRotation;
    private float RecoilX, RecoilY;
    private float ReturnSpeed;
    private float Snapiness;

    // Start is called before the first frame update
    public void CameraRecoilValues(float _RecoilX, float _RecoilY, float _Snapiness, float returnSpeed)
    {
        RecoilX = _RecoilX;
        RecoilY = _RecoilY;
        ReturnSpeed = returnSpeed;
        Snapiness = _Snapiness;
    }

    // Update is called once per frame
    void Update()
    {
        TargetRotation = Vector3.Lerp(TargetRotation, Vector3.zero, ReturnSpeed * Time.deltaTime);
        CurrentRotation = Vector3.Slerp(CurrentRotation, TargetRotation, Snapiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(CurrentRotation);
    }
    public void RecoilFire()
    {
        TargetRotation += new Vector3(RecoilX, Random.Range(-RecoilY, RecoilY), 0);
    }
}
