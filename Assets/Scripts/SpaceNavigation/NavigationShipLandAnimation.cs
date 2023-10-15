//using DG.Tweening;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class NavigationShipLandAnimation : MonoBehaviour
//{

//    NavigationShip navigationShip;

//    void Start()
//    {
//        navigationShip = GetComponent<NavigationShip>();
//    }

    
//    private void ShipAnimation()
//    {
//        if (navigationShip == null)
//        {
//            CloseAndSetScene();
//            return;
//        }

//        navigationShip.LockControls();

//        var sequence = DOTween.Sequence();

//        sprite.color = Color.white;

//        if (OnSelectAKEvent != null)
//            OnSelectAKEvent.Post(gameObject);
        
//        // -- Feedback de seleção e UI de autopilot         
//        sequence.Append(selector.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.22f).OnComplete(() => navSceneManager.BlinkAutoPilot()));
//        sequence.AppendInterval(0.1f);
        
//        float x = 0f;
//        float shipStartRotation = navigationShip.spritesTransform.eulerAngles.z + 90f;
//        Vector2 startPos = navigationShip.transform.position;

//        NavigationShipSoundController shipSoundController = navigationShip.GetComponent<NavigationShipSoundController>();
        
//        sequence.AppendCallback( () => 
//        {
//            if (shipSoundController)
//            {
//                navigationShip.SetFlightStepsSound(true);
//                navigationShip.OverridenControls = Vector2.up;
//                navigationShip.OverrideMode = true;
//            }
//        } );

//        // -- Animação curta
//        if (Vector2.Distance(startPos, transform.position) < 0.7f)
//        {
//            var angle = Mathg.AngleOfTheLineBetweenTwoPoints(new Vector2(transform.position.x, transform.position.y), startPos) - 180f;
//            sequence.AppendCallback( () => 
//            {
//                navigationShip.SetFlightStepsSound(true);
//                if (shipSoundController)
//                {
//                    this.WaitSeconds(.1f, () => shipSoundController.ReadInput(Vector2.up, intensity: 1.84f) );
//                }
//            });
//            sequence.Append( navigationShip.spritesTransform.DORotate(new Vector3(0f,0f, angle),1.3f));
//            sequence.Join(navigationShip.transform.DOMove(transform.position, 1.3f).OnComplete(
//                () =>
//                {
//                    // -- Mostra UI de aterrisagem no final
//                    navSceneManager.StopBlinkAutoPilot();
//                    navSceneManager.DisplayLandingSign();    

//                    navigationShip.SetFlightStepsSound(false);
//                    navigationShip.OverridenControls = Vector2.zero;
                    
//                    if (OnReachDestinationAKEvent != null)
//                        OnReachDestinationAKEvent.Post(gameObject);
//                }));
//        }
//        // -- Animação longa
//        else
//        {
//            SetCurve(shipStartRotation, startPos, transform.position);
        
//            sequence.Append(DrawDots(.3f, 10, shipStartRotation, startPos, transform.position));

//            sequence.AppendInterval(0.35f);
        
//            sequence.AppendCallback( () => 
//            {
//                navigationShip.SetFlightStepsSound(true);
//                if (shipSoundController)
//                {
//                    this.WaitSeconds(.1f, () => shipSoundController.ReadInput(Vector2.up, intensity: 1.84f) );
//                }
//            });

//            // -- Tween da movimentação e rotação
//            sequence.Append(DOVirtual.Float(0f, 1f, 2.75f, value =>
//                {
//                    navigationShip.transform.position = animationBezier.GetPoint(value);
//                    navigationShip.spritesTransform.eulerAngles = new Vector3(0f, 0f,
//                        animationBezier.GetRotationAtPoint(value));
//                }).SetEase(Ease.OutFlash)
//                .OnComplete(() =>
//                {
//                    // -- Mostra UI de aterrisagem no final
//                    navSceneManager.StopBlinkAutoPilot();
//                    navSceneManager.DisplayLandingSign();

//                    navigationShip.SetFlightStepsSound(false);
//                    navigationShip.OverridenControls = Vector2.zero;

//                    if (OnReachDestinationAKEvent != null)
//                        OnReachDestinationAKEvent.Post(gameObject);
//                })
//            );    
//        }
        
//        sequence.AppendInterval(2.5f);

//        //Setta Cena e fecha
//        sequence.OnComplete(CloseAndSetScene);

//    }
    

//    private Tween DrawDots(float duration, int count, float startRotation, Vector2 startPos, Vector2 endPos)
//    {
//        var s = DOTween.Sequence();

//        if (OnSelectAKEvent != null)
//            OnSelectAKEvent.Post(gameObject);

//        float c = count;
        
//        for (int i = 0; i < count; i++)
//        {
//            int index = i;
//            s.Append(DOVirtual.DelayedCall(duration / c, () =>
//            {
//                var pos = animationBezier.GetNormalizedPoint(index / c);
//                var dot = dotsParent.GetChild(index);
//                dot.position = pos;
                
//                if (MakePathAKEvent != null)
//                    MakePathAKEvent.Post(gameObject);
                
//            }));
//        }

//        return s;
//    }

//    private void SetCurve(float startRotation, Vector2 startPos, Vector2 endPos)
//    {
//        Vector2 p2 = startPos + new Vector2(Mathg.AngleToDirection(startRotation).x,Mathg.AngleToDirection(startRotation).y) * p2Lenght;
//        p2Debug = p2;
        
//        float side = (startPos.y < endPos.y) ? -1f : 1f;
        
//        Vector2 p3 = endPos + Vector2.up * p3Lenght * side;
//        p3Debug = p3;
        
//        animationBezier = new Curve(startPos, p2, p3, endPos);
//    }
//}
