using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour {
	

	const float scale = 2.5f;

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

	public LODInfo[] detailLevels;
	public SpawnableObject[] spawnableObjects;
	public static float maxViewDst;

	public Transform viewer;
	public Material mapMaterial;
	public static Vector2 viewerPosition;
	Vector2 viewerPositionOld;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDst;
	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start() {
		mapGenerator = FindObjectOfType<MapGenerator> ();

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

		UpdateVisibleChunks ();
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z) / scale;

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
		}
	}
		
	void UpdateVisibleChunks() {

		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					// terrainChunkDictionary [viewedChunkCoord].PlaceObjectsOnTerrain();
				} else {
					terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial, this));
				}

			}
		}
	}

	public class TerrainChunk {
		// GameObject treePrefab;  // Assign in the inspector or load dynamically
		// GameObject rockPrefab;		 // Assign in the inspector or load dynamically
		GameObject meshObject; // Assign in the inspector or load dynamically

    // Add this line
	    EndlessTerrain endlessTerrain;
		Vector2 position;
		Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;

		LODInfo[] detailLevels;
		LODMesh[] lodMeshes;
		LODMesh collisionLODMesh;

		MapData mapData;
		bool mapDataReceived;
		int previousLODIndex = -1;

		public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, EndlessTerrain endlessTerrain) {
			// this.treePrefab = treePrefab;
        	// this.rockPrefab = rockPrefab;
			this.detailLevels = detailLevels;
			this.endlessTerrain = endlessTerrain;

			position = coord * size;
			bounds = new Bounds(position,Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x,0,position.y);

			meshObject = new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();
			meshRenderer.material = material;

			meshObject.transform.position = positionV3 * scale;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one * scale;
			SetVisible(false);

			lodMeshes = new LODMesh[detailLevels.Length];
			for (int i = 0; i < detailLevels.Length; i++) {
				lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
				if (detailLevels[i].useForCollider) {
					collisionLODMesh = lodMeshes[i];
				}
			}

			mapGenerator.RequestMapData(position,OnMapDataReceived);
		}

		void OnMapDataReceived(MapData mapData) {
			this.mapData = mapData;
			mapDataReceived = true;

			Texture2D texture = TextureGenerator.TextureFromColourMap (mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
			meshRenderer.material.mainTexture = texture;

			UpdateTerrainChunk ();
		}

		public void PlaceObjectsOnTerrain() {
			// Loop through the vertices or grid of your chunk to place objects
			
			Mesh mesh = meshFilter.sharedMesh;
			Vector3[] vertices = mesh.vertices; // Get vertices of the terrain mesh
			

			// Loop through the vertices to place objects based on the height of the terrain
			for (int i = 0; i < vertices.Length; i++) {
				// Get the current vertex position
				Vector3 vertexPosition = vertices[i];
				float height = vertexPosition.y;

				// float normalizedHeight = (height - yMin) / (yMax - yMin);

				// Convert from local to world space to position objects properly
				Vector3 worldPosition = meshObject.transform.TransformPoint(vertexPosition);
				// Get the world-space height (Y value in Unity's coordinate system)
				float worldHeight = worldPosition.y;
				// Generate a random rotation for the objects
        		Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				

				 // Loop through each object type and check its spawn condition
                foreach (SpawnableObject spawnableObject in endlessTerrain.spawnableObjects) {
                    if (worldHeight >= spawnableObject.minHeight && worldHeight <= spawnableObject.maxHeight) {
                        if (Random.value < spawnableObject.density) {
                            GameObject instance = Instantiate(spawnableObject.prefab, worldPosition, randomRotation);
                            instance.transform.parent = meshObject.transform;
                        }
                    }
                }
				
			}
		}

		public void UpdateTerrainChunk() {
			if (mapDataReceived) {
				float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));
				bool visible = viewerDstFromNearestEdge <= maxViewDst;

				if (visible) {
					int lodIndex = 0;

					for (int i = 0; i < detailLevels.Length - 1; i++) {
						if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) {
							lodIndex = i + 1;
						} else {
							break;
						}
					}

					if (lodIndex != previousLODIndex) {
						LODMesh lodMesh = lodMeshes [lodIndex];
						if (lodMesh.hasMesh) {
							previousLODIndex = lodIndex;
							meshFilter.mesh = lodMesh.mesh;
							PlaceObjectsOnTerrain();
						} else if (!lodMesh.hasRequestedMesh) {
							lodMesh.RequestMesh (mapData);
						}
					}

					if (lodIndex == 0) {
						if (collisionLODMesh.hasMesh) {
							meshCollider.sharedMesh = collisionLODMesh.mesh;
						} else if (!collisionLODMesh.hasRequestedMesh) {
							collisionLODMesh.RequestMesh (mapData);
						}
					}

					terrainChunksVisibleLastUpdate.Add (this);
				}
				
				SetVisible (visible);
			}
		}

		public void SetVisible(bool visible) {
			meshObject.SetActive (visible);
		}

		public bool IsVisible() {
			return meshObject.activeSelf;
		}

	}

	class LODMesh {

		public Mesh mesh;
		public bool hasRequestedMesh;
		public bool hasMesh;
		int lod;
		System.Action updateCallback;

		public LODMesh(int lod, System.Action updateCallback) {
			this.lod = lod;
			this.updateCallback = updateCallback;
		}

		void OnMeshDataReceived(MeshData meshData) {
			mesh = meshData.CreateMesh ();
			hasMesh = true;

			updateCallback ();
		}

		public void RequestMesh(MapData mapData) {
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData (mapData, lod, OnMeshDataReceived);
		}

	}

	[System.Serializable]
	public struct LODInfo {
		public int lod;
		public float visibleDstThreshold;
		public bool useForCollider;
	}
	[System.Serializable]
	public struct SpawnableObject {
		public string name;           // Name of the object
		public GameObject prefab;      // Prefab of the object
		public float minHeight;        // Minimum height for spawning
		public float maxHeight;        // Maximum height for spawning
		public float density;          // Spawn density
	}


}
