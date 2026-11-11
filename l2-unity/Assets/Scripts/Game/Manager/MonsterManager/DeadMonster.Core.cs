using UnityEngine;

public partial class DeadMonster : MonoBehaviour
{
    void Update()
    {
        if (_useAntiGravity)
        {
            float newY3 = transform.position.y;
            float newY4 = newY3 * 2;
            float zeroPosition = newY3 - newY4;
            currentPos = zeroPosition - zeroPosition * 2;
            zeroPos = currentPos;
            currentPos = currentPos + 0.5f;
            float lerp = Mathf.Lerp(transform.position.y, 0.5f, speed * Time.deltaTime);
            _monsterRenderer = gameObject.GetComponentsInChildren<Renderer>();
            _isRefresh = true; 
            _useAntiGravity = false;
            NameplatesManager nameplates = NameplatePacketServices.Nameplates;
            if (nameplates != null)
                nameplates.Remove(_monster_id);
        }
        else
        {
            if (_isRefresh)
            {
                if (transform.position.y <= 3)
                {
                    originalColor = GetOriginalColor(_monsterRenderer);
                    if(originalColor.a > 0.1)
                    {
                        float lerp = Mathf.Lerp(transform.position.y, currentPos, speed * Time.deltaTime);
                        float alpha = Mathf.Lerp(originalColor.a, 0f, 1f * Time.deltaTime);
                        transform.position = new Vector3(transform.position.x, lerp, transform.position.z);
                        SetOpacity(_monsterRenderer, alpha);
                    }
                    else
                    {
                        if (_world != null) _world.RemoveObject(_monster_id);
                        _isRefresh = false;
                    }
                   
                }

            }
            
        }
    }

    private void SetOpacity(Renderer[] monsterRenderer , float opacity)
    {
        if(monsterRenderer[0] != null)
        {
            Material material = monsterRenderer[0].material;
            if (material != null)
            {
                // need change setting material manual _Surface = 1, RenderType = Transperenty
                material.SetFloat("_Surface", 1); // 1 = Transparent, 0 = Opaque
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                // Изменяем цвет и устанавливаем прозрачность
                Color color = material.color;
                color.a = opacity; 
                material.color = color;
            }
        }
      
    }
}
