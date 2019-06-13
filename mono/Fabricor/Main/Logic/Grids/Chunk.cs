﻿using System;
using System.Collections.Generic;
using Fabricor.Main.Rendering;
using Fabricor.Main.Rendering.Loading;
using Fabricor.Main.Rendering.Models;

namespace Fabricor.Main.Logic.Grids
{
    public class Chunk
    {
        public ushort[,,] blocks=new ushort[16,16,16];
        public TexturedModel model { get; private set; }
        public int xCoord = 0, yCoord = 0, zCoord = 0;
        public bool ShouldUpdate = false;

        public Chunk(int xCoord, int yCoord, int zCoord)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.zCoord = zCoord;
            model = new TexturedModel(MasterRenderer.GlLoader.LoadToDynamicVAO(new float[0], new float[0], new int[0]), BlockLookup.AtlasTexture);
        }

        private int Value(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= 16 || y >= 16 || z >= 16)
            {
                return 0;
            }
            return blocks[x, y, z];
        }

        public void UpdateModel()
        {
            ShouldUpdate = false;
            List<float> verts = new List<float>();
            List<float> texCoords = new List<float>();
            List<int> indices = new List<int>();
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        if (Value(x - 1, y, z) > 0 && Value(x + 1, y, z) > 0 && Value(x, y - 1, z) > 0
                        && Value(x, y + 1, z) > 0 && Value(x, y, z - 1) > 0 && Value(x, y, z + 1) > 0)
                            continue;

                        if (Value(x, y, z) <= 0)
                            continue;

                        int vcount = verts.Count/3;
                        Mesh m = BlockLookup.GetBlockMesh(blocks[x,y,z]);
                        for (int i = 0; i < m.vertices.Length; i++)
                        {
                            verts.Add(m.vertices[i] + x);
                            i++;
                            verts.Add(m.vertices[i] + y);
                            i++;
                            verts.Add(m.vertices[i] + z);
                        }
                        for (int i = 0; i < m.texCoords.Length; i++)
                        {
                            texCoords.Add(m.texCoords[i]);
                            i++;
                            texCoords.Add(m.texCoords[i]);
                        }
                        for (int i = 0; i < m.indices.Length; i++)
                        {
                            indices.Add(m.indices[i] + vcount);
                        }
                    }
                }
            }

            MasterRenderer.GlLoader.UpdateDynamicVAO((DynamicModel)model.RawModel, 0, verts.ToArray(), 3);
            MasterRenderer.GlLoader.UpdateDynamicVAO((DynamicModel)model.RawModel, 1, texCoords.ToArray(), 2);
            MasterRenderer.GlLoader.UpdateDynamicVAO((DynamicModel)model.RawModel, indices.ToArray());
        }

        
    }
}