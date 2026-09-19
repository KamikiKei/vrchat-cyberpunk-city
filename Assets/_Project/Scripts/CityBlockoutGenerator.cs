using UnityEngine;

public class CityBlockoutGenerator : MonoBehaviour
{
    [Header("Generated Content Root")]
    [Tooltip("生成物だけを格納する CityGeometry を指定します。未指定時はこのオブジェクト自身を使用します。")]
    [SerializeField] private Transform cityGeometryRoot;

    [Header("Generation Settings")]
    public GameObject blockPrefab; // プレハブ（未設定ならCubeが自動生成されます）
    public int gridWidth = 7;      // 横の区画数
    public int gridDepth = 7;      // 縦の区画数
    
    [Header("Layout Spacing")]
    public float buildingSize = 20.0f; // ビルの太さ（以前は8.0fだったのをドカンと広げる）
    public float roadWidth = 10.0f;    // 通路・道路の幅（ゆったりした大通りにする）
    
    [Header("Height Settings")]
    public float minHeight = 40f;  // 最低でも40m（以前は10f）
    public float maxHeight = 120f; // 最高で120m級の超高層ビル（以前は40f）

    [ContextMenu("Generate City Blockout")]
    public void GenerateCity()
    {
        Transform generatedRoot = GetGeneratedRoot();

        // CityGeometry 配下の生成物だけをクリアし、SpawnPoints / WorldSystems は保護する
        int childCount = generatedRoot.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(generatedRoot.GetChild(i).gameObject);
        }

        float totalCellSize = buildingSize + roadWidth;
        int centerIndexX = gridWidth / 2;
        int centerIndexZ = gridDepth / 2;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                // 中央（センター付近）は「中央駅・広場」スペースとしてビルを生成しない
                bool isCenterStation = (x >= centerIndexX - 1 && x <= centerIndexX + 1) && 
                                       (z >= centerIndexZ - 1 && z <= centerIndexZ + 1);

                if (isCenterStation)
                {
                    // 中央駅のベース（低くて広い建物やプラットフォーム）を配置する場合
                    continue; // ここでは一旦スペースとして空ける
                }

                // 座標計算（道路の幅を考慮）
                float posX = x * totalCellSize;
                float posZ = z * totalCellSize;
                Vector3 spawnPos = new Vector3(posX, 0, posZ);

                GameObject block;
                if (blockPrefab != null)
                {
                    block = Instantiate(blockPrefab, generatedRoot);
                }
                else
                {
                    block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.transform.SetParent(generatedRoot);
                }

                // ランダムな高層ビルを生成
                float randomHeight = Random.Range(minHeight, maxHeight);
                block.transform.localScale = new Vector3(buildingSize, randomHeight, buildingSize);
                block.transform.position = new Vector3(posX, randomHeight / 2f, posZ);
            }
        }

        // 中央駅の代わりの広場マーカー（床）を生成
        CreateCentralPlaza(generatedRoot, centerIndexX, centerIndexZ, totalCellSize);

        Debug.Log("中央駅と道路スペースを確保した街の生成が完了しました！");
    }

    private Transform GetGeneratedRoot()
    {
        return cityGeometryRoot != null ? cityGeometryRoot : transform;
    }

    void CreateCentralPlaza(Transform generatedRoot, int centerX, int centerZ, float totalCellSize)
    {
        // 広場の床を作成
        GameObject plaza = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plaza.name = "CentralStation_Plaza";
        plaza.transform.SetParent(generatedRoot);

        float plazaSize = totalCellSize * 3.0f - roadWidth;
        float centerXPos = centerX * totalCellSize;
        float centerZPos = centerZ * totalCellSize;

        plaza.transform.position = new Vector3(centerXPos, 0.1f, centerZPos);
        plaza.transform.localScale = new Vector3(plazaSize, 0.2f, plazaSize);

        // 見た目を区別しやすくするために色を変える（マテリアルがあれば変更可能ですが今回は簡易的に）
        Renderer rend = plaza.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial.color = new Color(0.3f, 0.3f, 0.3f); // 灰色っぽい広場
        }
    }
}
