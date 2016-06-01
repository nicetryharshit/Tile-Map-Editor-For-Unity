using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow 
{
	GameObject[] prefabs;
	GameObject selectedPrefab;
	List<GameObject> spawnedGO = new List<GameObject>();
	GUIContent handleContent = new GUIContent();

	[MenuItem("Window/Map Editor")]
	static void Init()
	{

		var window = (MapEditor)GetWindow(typeof(MapEditor), false, "Map Editor");
		window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 500f, 250f);
	}


	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;

	}
	void OnGUI()
	{

		//Load all prefabs as objects from the 'Prefabs' folder
		Object[] obj =  Resources.LoadAll ("Prefabs",typeof(GameObject));

		//initialize the game object array
		prefabs = new GameObject[obj.Length];

		//store the game objects in the array
		for(int i=0;i<obj.Length;i++)
		{
			prefabs[i]=(GameObject)obj[i];
		}

		GUILayout.BeginHorizontal ();

        if(prefabs!=null)
        {
            int elementsInThisRow=0;

            for( int i=0; i<prefabs.Length; i++)
            {
                elementsInThisRow++;

                //get the texture from the prefabs
                Texture prefabTexture = prefabs[i].GetComponent<SpriteRenderer> ().sprite.texture;

                //create one button for earch prefabs 
                //if a button is clicked, select that prefab and focus on the scene view
                if(GUILayout.Button(prefabTexture,GUILayout.MaxWidth(50), GUILayout.MaxHeight(50)))
                {    
                    selectedPrefab = prefabs[i];
					handleContent.image = (Texture2D)selectedPrefab.GetComponent<SpriteRenderer>().sprite.texture;

					SceneView.onSceneGUIDelegate -= OnSceneGUI;

					SceneView.onSceneGUIDelegate += OnSceneGUI;
					EditorWindow.FocusWindowIfItsOpen<SceneView>();

				

                }

                //move to next row after creating a certain number of buttons so it doesn't overflow horizontally
                if(elementsInThisRow>Screen.width/70)
                {
                    elementsInThisRow=0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal ();        
                }
            }
        }
        GUILayout.EndHorizontal();
	}

	void OnSceneGUI(SceneView sceneView)
	{
		Handles.BeginGUI();

		GUILayout.Box(handleContent, GUILayout.MinWidth(40), GUILayout.MinHeight(40));
		if(GUILayout.Button("Done", GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
		}

		Handles.EndGUI();     

		Vector3	spawnPosition = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin;

	    //if 'E' pressed, spawn the selected prefab
        if (Event.current.type==EventType.KeyDown &&Event.current.keyCode == KeyCode.E) 
        {
 			Spawn(spawnPosition);
        }


		//if 'X' is pressed, undo (remove the last spawned prefab)
		if (Event.current.type==EventType.KeyDown &&Event.current.keyCode == KeyCode.X) 
		{	
			if(spawnedGO.Count>0)
			{
				if(spawnedGO[spawnedGO.Count-1]!=null)
				{
					DestroyImmediate(spawnedGO[spawnedGO.Count-1]);
				}
				spawnedGO.RemoveAt(spawnedGO.Count-1);
			}
		}


		if(selectedGameObject!=null)
        {
            Handles.Label(selectedGameObject.transform.position, "X");

        }

	

		if(selectedPrefab!=null)
		{
			if(selectedGameObject!=null && selectedGameObject.GetComponent<SpriteRenderer>())
			{
				float selectedGameObjectWidth = selectedGameObject.GetComponent<SpriteRenderer>().bounds.size.x;
				float selectedGameObjectHeight = selectedGameObject.GetComponent<SpriteRenderer>().bounds.size.y;

				float selectedPrefabWidth = selectedPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
				float selectedPrefabHeight = selectedPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

				if (Event.current.type==EventType.KeyDown &&Event.current.keyCode == KeyCode.W) 
				{
					spawnPosition = new Vector3(selectedGameObject.transform.position.x, selectedGameObject.transform.position.y+(selectedGameObjectHeight/2)+(selectedPrefabHeight/2), 0);
					Spawn(spawnPosition);
				}
				if (Event.current.type==EventType.KeyDown &&Event.current.keyCode == KeyCode.S) 
				{
					spawnPosition = new Vector3(selectedGameObject.transform.position.x, selectedGameObject.transform.position.y-(selectedGameObjectHeight/2)-(selectedPrefabHeight/2), 0);
					Spawn(spawnPosition);
				}
				if (Event.current.type==EventType.KeyDown &&Event.current.keyCode == KeyCode.A) 
				{
					spawnPosition = new Vector3(selectedGameObject.transform.position.x-(selectedGameObjectWidth/2)-(selectedPrefabWidth/2), selectedGameObject.transform.position.y, 0);
					Spawn(spawnPosition);
				}
				if (Event.current.type==EventType.KeyDown &&Event.current.keyCode == KeyCode.D) 
				{
					spawnPosition = new Vector3(selectedGameObject.transform.position.x+(selectedGameObjectWidth/2)+(selectedPrefabWidth/2), selectedGameObject.transform.position.y, 0);
					Spawn(spawnPosition);
				}
	
			}
		}

		if(Selection.activeGameObject!=null)
		{
			selectedGameObject = Selection.activeGameObject;
		}

		SceneView.RepaintAll();
	}

	GameObject selectedGameObject;
	void Spawn(Vector2 _spawnPosition)
    {
        GameObject go = (GameObject)Instantiate(selectedPrefab,new Vector3(_spawnPosition.x, _spawnPosition.y, 0), selectedPrefab.transform.rotation);
        Selection.activeObject = go;

		go.name = selectedPrefab.name;
        
        spawnedGO.Add(go);
        if(spawnedGO.Count>49)
        {
        	spawnedGO.RemoveAt(0);
        }
    }
}
