# Mountain-Generator
this project proceduraly generates mountain ranges with perlin noise, each mountain range consists of multiple mesh chunks. Peaks of generated mountain range are highlighted with peak indicators

Mountain Object must contain these scripts:
  - MapGenerator
  - MeshGenerator
  - PeakFinder
  - SideWallGenerator
  - RotationWithSlider (UI) 

MapGenerator = main script containing all settings to create mountain:
  - ChunkPrefab = GameObject with MeshRenderer, MeshFilter, MeshGenerator script and MeshCollider
  - ChunkCountInLine = how many chunks create edge of rectangular mountain range
  - ChunkSize = size of each chunk (segment) of mountain range
  - WorldSeed = seed used to generate mountain
  - MountaingHeight = max height mountain can achieve
  - Scale, Octaves, Persistance, Lacunarity = setting for generation of perlin noise
  - DetailLevel = how high level of detail (vertices) each chunk holds
  - Curve = sampling by height of each point, the points height is multiplied by sampled value on curve
  - Gradiend = collor gradient used by mountain material
  - AutoUpdate = will generate new mountain range for each change in these settings
  - CustomSeed = mountain generator will not change seed if CustomSeed = false

MeshGenerator = script that creates mesh for each chunk

PeakFinder = script that finds significant peaks and indicates them

SideWallGenerator = creates mesh around rectangular mountain range map

RotationWithSlider = allows to rotate map using sliders
