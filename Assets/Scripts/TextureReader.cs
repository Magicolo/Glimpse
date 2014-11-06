using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TextureReader : MonoBehaviour {
	// -- TerrainSurface.cs --

	public Terrain gameTerrain;
	public Transform playerTransform;
	
	static readonly Dictionary<int, AudioMaster.FootstepSurfaces> indexTextureDict = new Dictionary<int, AudioMaster.FootstepSurfaces> {
		{ 0, AudioMaster.FootstepSurfaces.Dirt },
		{ 1, AudioMaster.FootstepSurfaces.Grass },
		{ 2, AudioMaster.FootstepSurfaces.Stone },
		{ 3, AudioMaster.FootstepSurfaces.Dirt },
		{ 4, AudioMaster.FootstepSurfaces.Stone },
		{ 5, AudioMaster.FootstepSurfaces.Dirt },
		{ 6, AudioMaster.FootstepSurfaces.Dirt },
		{ 7, AudioMaster.FootstepSurfaces.Grass },
		{ 8, AudioMaster.FootstepSurfaces.Dirt },
		{ 9, AudioMaster.FootstepSurfaces.Stone },
		{ 10, AudioMaster.FootstepSurfaces.Dirt }
	};
	
	static TextureReader instance;
	static TextureReader Instance {
		get {
			instance = instance ?? FindObjectOfType<TextureReader>();
			return instance;
		}
	}
	
	void Update() {
		// Debug.Log ( GetMainTexture () );
	}

	public float[] GetTextureMix() {
			
		// returns an array containing the relative mix of textures
		// on the main terrain at this world position.
			
		// The number of values in the array will equal the number
		// of textures added to the terrain.
			
		Vector3 playerPosition = playerTransform.position;
		TerrainData terrainData = gameTerrain.terrainData;
		Vector3 terrainPos = gameTerrain.transform.position;
			
		// calculate which splat map cell the worldPos falls within (ignoring y)
		int mapX = (int)(((playerPosition.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
		int mapZ = (int)(((playerPosition.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
			
		// get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
		float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
			
		// extract the 3D array data to a 1D array:
		float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
		for (int n = 0; n < cellMix.Length; ++n) {
			cellMix[n] = splatmapData[0, 0, n];    
		}
			
		return cellMix;        
			
	}
		
	public int GetMainTextureIndex() {
			
		// returns the zero-based index of the most dominant texture
		// on the main terrain at this world position.
			
		float[] mix = GetTextureMix();
			
			
		float maxMix = 0;
		int maxIndex = 0;
			
		// loop through each mix value and find the maximum
		for (int n = 0; n < mix.Length; ++n) {
			if (mix[n] > maxMix) {
				maxIndex = n;
				maxMix = mix[n];
			}
		}
			
		return maxIndex;
			
	}
	
	/// <summary>
	/// Gets the most dominant texture name under the player.
	/// </summary>
	/// <returns>The texture name.</returns>
	public static AudioMaster.FootstepSurfaces GetMainTexture(){
		return indexTextureDict[Instance.GetMainTextureIndex()];
	}
}
