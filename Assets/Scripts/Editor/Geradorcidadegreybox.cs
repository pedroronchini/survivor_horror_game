// Coloque este arquivo em Assets/Scripts/Editor/
// No Unity: menu Ferramentas > Gerar Cidade Greybox
//
// Cria a cena Assets/Scenes/Cidade_Greybox.unity com ruas, calçadas,
// prédios (blocos simples), bloqueios e muros que fecham os vãos, seguindo a
// planta "Mapa da Cidade Abandonada". 1 unidade = 1 metro.
// X = leste, Z = norte, origem (0, 0) no canto sudoeste do mapa.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GeradorCidadeGreybox
{
    private const string CaminhoCena = "Assets/Scenes/Cidade_Greybox.unity";
    private const string PastaMateriais = "Assets/Materials/Greybox";

    // Tamanho do mapa em metros
    private const int Largura = 150; // X
    private const int Profundidade = 125; // Z

    // Alturas
    private const float AlturaCalcada = 0.15f; // altura do meio-fio
    private const float FundoChao = -0.5f;     // base das lajes do chão

    // Tipos de célula do chão
    private const int Calcada = 0;
    private const int Rua = 1;
    private const int Porto = 2;

    private static readonly Dictionary<string, Material> materiais = new Dictionary<string, Material>();

    [MenuItem("Ferramentas/Gerar Cidade Greybox")]
    public static void Gerar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(CaminhoCena) != null &&
            !EditorUtility.DisplayDialog("Cidade Greybox",
                "A cena Cidade_Greybox já existe. Quer substituir?", "Substituir", "Cancelar"))
            return;

        CriarMateriais();

        var cena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var raiz = new GameObject("Cidade").transform;

        GerarChao(raiz);
        GerarPredios(raiz);
        GerarBloqueios(raiz);
        GerarFechamento(raiz);
        GerarMurosDeContorno(raiz);
        GerarMarcadores(raiz);
        PosicionarCamera();

        EditorSceneManager.SaveScene(cena, CaminhoCena);
        AssetDatabase.Refresh();
        Debug.Log("Cidade greybox criada em " + CaminhoCena);
    }

    // ---------------------------------------------------------------
    // Chão: ruas no nível 0 e calçadas 0,15 m acima
    // ---------------------------------------------------------------

    private static void GerarChao(Transform raiz)
    {
        // Grade de 1 x 1 m. Tudo começa como calçada/terreno.
        int[,] grade = new int[Largura, Profundidade];

        // Pistas de rolamento (rua sem as calçadas de 2 m de cada lado)
        MarcarArea(grade, 18, 150, 47, 53, Rua);   // Rua Sul + estrada da usina
        MarcarArea(grade, 57, 63, 22, 108, Rua);   // Estrada do porto + Avenida Principal
        MarcarArea(grade, 26, 128, 87, 93, Rua);   // Rua Norte
        MarcarArea(grade, 45, 75, 2, 22, Porto);   // Pátio de pouso

        var pai = CriarGrupo("Chao", raiz);
        var rCalcada = CriarGrupo("Calcadas", pai);
        var rRua = CriarGrupo("Ruas", pai);
        var rPorto = CriarGrupo("Porto", pai);

        int contador = 0;
        foreach (int tipo in new[] { Calcada, Rua, Porto })
        {
            float topo = tipo == Calcada ? AlturaCalcada : 0f;
            Transform grupo = tipo == Calcada ? rCalcada : (tipo == Rua ? rRua : rPorto);
            string mat = tipo == Calcada ? "Calcada" : (tipo == Rua ? "Asfalto" : "Porto");

            foreach (var r in JuntarRetangulos(grade, tipo))
                CriarBloco(grupo.name + "_" + contador++, grupo, r.x, r.xMax, r.y, r.yMax, FundoChao, topo, mat);
        }
    }

    private static void MarcarArea(int[,] grade, int x1, int x2, int z1, int z2, int tipo)
    {
        for (int x = Mathf.Max(0, x1); x < Mathf.Min(Largura, x2); x++)
            for (int z = Mathf.Max(0, z1); z < Mathf.Min(Profundidade, z2); z++)
                grade[x, z] = tipo;
    }

    // ---------------------------------------------------------------
    // Prédios (blocos sólidos por enquanto; interiores vêm depois)
    // ---------------------------------------------------------------

    private static void GerarPredios(Transform raiz)
    {
        var pai = CriarGrupo("Predios", raiz);

        var interior = CriarGrupo("ComInterior", pai);
        CriarPredio("Loja", interior, 40, 54, 58, 74, 4f, "Interior");
        CriarPredio("Pensao", interior, 67, 87, 60, 80, 7f, "Interior");
        CriarPredio("Empresa_Robos", interior, 96, 124, 98, 122, 12f, "Interior");
        CriarPredio("Cafeteria", interior, 30, 44, 97, 108, 4f, "Interior");

        var fachadas = CriarGrupo("Fachadas", pai);
        CriarPredio("F1_SulRuaSul_Oeste", fachadas, 22, 53, 28, 43, 8f, "Fachada");
        CriarPredio("F2_SulRuaSul_Leste", fachadas, 67, 126, 28, 43, 9f, "Fachada");
        CriarPredio("F3_OesteLoja", fachadas, 26, 38, 58, 82, 7f, "Fachada");
        CriarPredio("F4_NorteLoja", fachadas, 40, 54, 76, 82, 6f, "Fachada");
        CriarPredio("F5_LadoCafeteria", fachadas, 46, 54, 97, 108, 6f, "Fachada");
        CriarPredio("F6_NorteRuaNorte", fachadas, 66, 84, 97, 108, 8f, "Fachada");
        CriarPredio("F7_LesteBeco", fachadas, 92, 124, 58, 82, 9f, "Fachada");

        // Gradil do pátio da empresa (baixo, com abertura no portão)
        var patio = CriarGrupo("Patio_Empresa", pai);
        CriarPredio("Gradil_Oeste", patio, 96, 108, 95, 96, 1.5f, "Bloqueio");
        CriarPredio("Gradil_Leste", patio, 112, 124, 95, 96, 1.5f, "Bloqueio");
    }

    private static void CriarPredio(string nome, Transform pai, int x1, int x2, int z1, int z2, float altura, string mat)
    {
        CriarBloco(nome, pai, x1, x2, z1, z2, AlturaCalcada, AlturaCalcada + altura, mat);
    }

    // ---------------------------------------------------------------
    // Bloqueios
    // ---------------------------------------------------------------

    private static void GerarBloqueios(Transform raiz)
    {
        var pai = CriarGrupo("Bloqueios", raiz);
        CriarBloco("Desabado_OesteRuaSul", pai, 18, 22, 43, 58, 0f, 5f, "Bloqueio");
        CriarBloco("Desabado_FimAvenida", pai, 55, 65, 108, 114, 0f, 5f, "Bloqueio");
        // Os bloqueios da Rua Norte cobrem rua + calçadas para não dar para contornar
        CriarBloco("Carros_OesteRuaNorte", pai, 26, 29, 82, 97, 0f, 3f, "Bloqueio");
        CriarBloco("Barricada_Atalho", pai, 84, 87, 82, 97, 0f, 3f, "Atalho");
        CriarBloco("Escombros_LesteRuaNorte", pai, 124, 128, 85, 95, 0f, 5f, "Bloqueio");
        CriarBloco("Portao_Usina", pai, 128, 130, 45, 55, 0f, 4f, "Portao");
    }

    // ---------------------------------------------------------------
    // Fechamento: tudo que não é área andável nem prédio vira muro.
    // Assim o jogador não sai da rota por vãos entre os prédios.
    // ---------------------------------------------------------------

    private const int Livre = 0;
    private const int Andavel = 1;
    private const int Ocupado = 2;

    private static void GerarFechamento(Transform raiz)
    {
        int[,] grade = new int[Largura, Profundidade];

        // Áreas andáveis (rua + calçadas + margens até as fachadas)
        MarcarArea(grade, 45, 75, 2, 22, Andavel);     // Porto de pouso
        MarcarArea(grade, 56, 64, 22, 45, Andavel);    // Estrada do porto
        MarcarArea(grade, 18, 128, 43, 58, Andavel);   // Rua Sul
        MarcarArea(grade, 128, 150, 47, 53, Andavel);  // Estrada da usina (depois do portão)
        MarcarArea(grade, 54, 67, 58, 85, Andavel);    // Avenida (entre as ruas)
        MarcarArea(grade, 54, 66, 95, 108, Andavel);   // Avenida (norte, até o desabamento)
        MarcarArea(grade, 26, 124, 82, 97, Andavel);   // Rua Norte
        MarcarArea(grade, 96, 124, 95, 98, Andavel);   // Pátio da empresa
        MarcarArea(grade, 87, 90, 60, 85, Andavel);    // Beco (só se chega pela pensão)

        // Prédios já ocupam o espaço deles
        MarcarArea(grade, 40, 54, 58, 74, Ocupado);
        MarcarArea(grade, 67, 87, 60, 80, Ocupado);
        MarcarArea(grade, 96, 124, 98, 122, Ocupado);
        MarcarArea(grade, 30, 44, 97, 108, Ocupado);
        MarcarArea(grade, 22, 53, 28, 43, Ocupado);
        MarcarArea(grade, 67, 126, 28, 43, Ocupado);
        MarcarArea(grade, 26, 38, 58, 82, Ocupado);
        MarcarArea(grade, 40, 54, 76, 82, Ocupado);
        MarcarArea(grade, 46, 54, 97, 108, Ocupado);
        MarcarArea(grade, 66, 84, 97, 108, Ocupado);
        MarcarArea(grade, 92, 124, 58, 82, Ocupado);

        var pai = CriarGrupo("Fechamento", raiz);
        int n = 0;
        foreach (var r in JuntarRetangulos(grade, Livre))
            CriarBloco("Muro_" + n++, pai, r.x, r.xMax, r.y, r.yMax, AlturaCalcada, 5f, "Muro");
    }

    // Junta células iguais em retângulos para criar poucos objetos
    private static List<RectInt> JuntarRetangulos(int[,] grade, int tipo)
    {
        var lista = new List<RectInt>();
        bool[,] usada = new bool[Largura, Profundidade];

        for (int z = 0; z < Profundidade; z++)
        {
            for (int x = 0; x < Largura; x++)
            {
                if (usada[x, z] || grade[x, z] != tipo) continue;

                int w = 1;
                while (x + w < Largura && !usada[x + w, z] && grade[x + w, z] == tipo) w++;

                int h = 1;
                bool podeCrescer = true;
                while (z + h < Profundidade && podeCrescer)
                {
                    for (int i = 0; i < w; i++)
                    {
                        if (usada[x + i, z + h] || grade[x + i, z + h] != tipo)
                        {
                            podeCrescer = false;
                            break;
                        }
                    }
                    if (podeCrescer) h++;
                }

                for (int dz = 0; dz < h; dz++)
                    for (int dx = 0; dx < w; dx++)
                        usada[x + dx, z + dz] = true;

                lista.Add(new RectInt(x, z, w, h));
            }
        }
        return lista;
    }

    private static void GerarMurosDeContorno(Transform raiz)
    {
        var pai = CriarGrupo("Muros_Contorno", raiz);
        const float altura = 6f;
        CriarBloco("Muro_Sul", pai, -1, Largura + 1, -1, 0, FundoChao, altura, "Fachada");
        CriarBloco("Muro_Norte", pai, -1, Largura + 1, Profundidade, Profundidade + 1, FundoChao, altura, "Fachada");
        CriarBloco("Muro_Oeste", pai, -1, 0, 0, Profundidade, FundoChao, altura, "Fachada");
        CriarBloco("Muro_Leste", pai, Largura, Largura + 1, 0, Profundidade, FundoChao, altura, "Fachada");
    }

    // ---------------------------------------------------------------
    // Marcadores (nave, painel do mapa e ponto de início do jogador)
    // ---------------------------------------------------------------

    private static void GerarMarcadores(Transform raiz)
    {
        var pai = CriarGrupo("Marcadores", raiz);
        CriarBloco("Nave_Robos", pai, 52, 68, 7, 17, 0f, 4f, "Fachada");
        CriarBloco("Painel_Mapa", pai, 66, 70, 56, 57, AlturaCalcada, AlturaCalcada + 2.2f, "Atalho");

        var spawn = new GameObject("SpawnJogador");
        spawn.transform.SetParent(pai);
        spawn.transform.position = new Vector3(70f, 0f, 12f);
        spawn.transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    private static void PosicionarCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.transform.position = new Vector3(75f, 140f, -20f);
        cam.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        cam.farClipPlane = 500f;
    }

    // ---------------------------------------------------------------
    // Utilitários
    // ---------------------------------------------------------------

    private static Transform CriarGrupo(string nome, Transform pai)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(pai);
        return go.transform;
    }

    // Cria um cubo a partir das bordas (x1..x2, z1..z2) e da altura (baseY..topoY)
    private static void CriarBloco(string nome, Transform pai, float x1, float x2, float z1, float z2,
        float baseY, float topoY, string mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = nome;
        go.transform.SetParent(pai);
        go.transform.position = new Vector3((x1 + x2) / 2f, (baseY + topoY) / 2f, (z1 + z2) / 2f);
        go.transform.localScale = new Vector3(x2 - x1, topoY - baseY, z2 - z1);
        go.GetComponent<MeshRenderer>().sharedMaterial = materiais[mat];
        go.isStatic = true;
    }

    private static void CriarMateriais()
    {
        materiais.Clear();
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder(PastaMateriais))
            AssetDatabase.CreateFolder("Assets/Materials", "Greybox");

        CriarMaterial("Asfalto", new Color(0.18f, 0.19f, 0.20f));
        CriarMaterial("Calcada", new Color(0.55f, 0.56f, 0.56f));
        CriarMaterial("Porto", new Color(0.32f, 0.34f, 0.35f));
        CriarMaterial("Fachada", new Color(0.42f, 0.45f, 0.48f));
        CriarMaterial("Interior", new Color(0.45f, 0.55f, 0.45f));
        CriarMaterial("Muro", new Color(0.30f, 0.32f, 0.34f));
        CriarMaterial("Bloqueio", new Color(0.50f, 0.32f, 0.27f));
        CriarMaterial("Atalho", new Color(0.85f, 0.62f, 0.22f));
        CriarMaterial("Portao", new Color(0.70f, 0.55f, 0.33f));
    }

    private static void CriarMaterial(string nome, Color cor)
    {
        string caminho = PastaMateriais + "/Greybox_" + nome + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(caminho);

        if (mat == null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, caminho);
        }

        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", cor);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", cor);
        EditorUtility.SetDirty(mat);

        materiais[nome] = mat;
    }
}