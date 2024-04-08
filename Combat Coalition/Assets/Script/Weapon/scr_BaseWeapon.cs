using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class scr_BaseWeapon : MonoBehaviour
{
    #region - Parameters - 
    //Rotation
    private Vector3 newWeaponRotation;
    private Vector3 newWeaponRotationVelocity;
    private Vector3 TargetWeaponRotation;
    private Vector3 TargetWeaponRotationVelocity;
    private Vector3 newWeaponMovementRotation;
    private Vector3 newWeaponMovementRotationVelocity;
    private Vector3 TargetWeaponMovementRotation;
    private Vector3 TargetWeaponMovementRotationVelocity;
    [SerializeField] scr_WeaponSO scr_Weapon;
    private Vector3 SwayPosition;
    protected scr_WeaponHolder holder;
    private float SwayTime;
    public bool IsAiming { get; protected set; }
    #endregion
    public virtual void Execute()
    {

    }

    #region - Start/Update/LateUpdate -
    private void LateUpdate()
    {
        CalculateWeaponRotation();
        CalculateWeaponSway();
    }
    #endregion

    #region - Sway -
    void CalculateWeaponSway()
    {
        var targetPos = Curve(SwayTime, holder.GetWeaponController().SwayAmountA, holder.GetWeaponController().SwayAmountB) / (IsAiming ? holder.GetWeaponController().SwayScale * 4 : holder.GetWeaponController().SwayScale);
        SwayPosition = Vector3.Lerp(SwayPosition, targetPos, Time.smoothDeltaTime * holder.GetWeaponController().SwayLerpSpeed);
        SwayTime += Time.deltaTime;
        if (SwayTime > 6.3f)
        {
            SwayTime = 0;
        }
        holder.GetWeaponController().SwayObj.localPosition = SwayPosition;
    }
    Vector3 Curve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
    #endregion

    #region - Rotation -

    void CalculateWeaponRotation()
    {
        TargetWeaponRotation.y += (IsAiming ? holder.GetWeaponController().Settings.SwayAmount / 2 : holder.GetWeaponController().Settings.SwayAmount) * (holder.GetWeaponController().Settings.SwayXInverted ? -holder.GetWeaponController().InputManeger.Input_View.x : holder.GetWeaponController().InputManeger.Input_View.x) * Time.deltaTime;
        TargetWeaponRotation.x += (IsAiming ? holder.GetWeaponController().Settings.SwayAmount / 2 : holder.GetWeaponController().Settings.SwayAmount) * (holder.GetWeaponController().Settings.SwayYInverted ? holder.GetWeaponController().InputManeger.Input_View.y : -holder.GetWeaponController().InputManeger.Input_View.y) * Time.deltaTime;
        TargetWeaponRotation.x = Mathf.Clamp(TargetWeaponRotation.x, -holder.GetWeaponController().Settings.SwayClampX, holder.GetWeaponController().Settings.SwayClampX);
        TargetWeaponRotation.y = Mathf.Clamp(TargetWeaponRotation.y, -holder.GetWeaponController().Settings.SwayClampY, holder.GetWeaponController().Settings.SwayClampY);
        TargetWeaponRotation.z = IsAiming ? 0 : TargetWeaponRotation.y;
        TargetWeaponRotation = Vector3.SmoothDamp(TargetWeaponRotation, Vector3.zero, ref TargetWeaponRotationVelocity, holder.GetWeaponController().Settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, TargetWeaponRotation, ref newWeaponRotationVelocity, holder.GetWeaponController().Settings.SwaySmoothing);
        TargetWeaponMovementRotation.z = (IsAiming ? holder.GetWeaponController().Settings.MovementSwayX / 2 : holder.GetWeaponController().Settings.MovementSwayX) * (holder.GetWeaponController().Settings.MovementSwayXInverted ? -holder.GetWeaponController().InputManeger.Input_Movement.x : holder.GetWeaponController().InputManeger.Input_Movement.x);
        TargetWeaponMovementRotation.x = (IsAiming ? holder.GetWeaponController().Settings.MovementSwayY / 2 : holder.GetWeaponController().Settings.MovementSwayY) * (holder.GetWeaponController().Settings.MovementSwayYInverted ? -holder.GetWeaponController().InputManeger.Input_Movement.y : holder.GetWeaponController().InputManeger.Input_Movement.y);
        TargetWeaponMovementRotation = Vector3.SmoothDamp(TargetWeaponMovementRotation, Vector3.zero, ref TargetWeaponMovementRotationVelocity, holder.GetWeaponController().Settings.SwayResetSmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, TargetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, holder.GetWeaponController().Settings.SwaySmoothing);
        holder.GetWeaponController().SwayObj.localRotation = Quaternion.Euler(newWeaponRotation - newWeaponMovementRotation);
    }
    #endregion
  
    private void SetWeaponHolder(scr_WeaponHolder scr_WeaponHolder)
    {
        if (scr_WeaponHolder.HasWeapon())
        {
            scr_WeaponHolder.DropWeapon();
        }
        holder = scr_WeaponHolder;
        holder.SetWeapon(this);
        holder.GetWeaponController().transform.localPosition = scr_Weapon.WeaponPos;
        var t = transform;
        t.SetParent(holder.GetWeaponParent(), false);
    }
    public static scr_BaseWeapon SpawnWeapon(scr_WeaponSO _WeaponSO, scr_WeaponHolder scr_WeaponHolder)
    {
        scr_BaseWeapon baseWeapon = Instantiate(_WeaponSO.Weapon);
        baseWeapon.SetWeaponHolder(scr_WeaponHolder);
        baseWeapon.gameObject.name = _WeaponSO.name;
        return baseWeapon;
    }
    public static scr_Pickable SpawnWeaponInWorld(scr_WeaponSO _WeaponSO, Vector3 position)
    {
        scr_Pickable WeaponPickable = Instantiate(_WeaponSO.WeaponPickable);
        WeaponPickable.transform.position = position;
        return WeaponPickable;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public scr_WeaponSO GetScr_WeaponSO()
    {
        return scr_Weapon;
    }
}
