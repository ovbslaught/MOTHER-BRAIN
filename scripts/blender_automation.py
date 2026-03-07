#!/usr/bin/env python3
"""
MOTHER-BRAIN Blender Automation Script
Headless Blender operations for 3D asset processing and generation
"""

import bpy
import os
import sys
import json
from pathlib import Path
import logging

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

class BlenderAutomation:
    """Automated Blender operations for MOTHER-BRAIN"""
    
    def __init__(self, input_dir="assets/input", output_dir="assets/output"):
        self.input_dir = Path(input_dir)
        self.output_dir = Path(output_dir)
        self.output_dir.mkdir(parents=True, exist_ok=True)
        logger.info("🎨 BlenderAutomation initialized")
    
    def clear_scene(self):
        """Clear all objects from the scene"""
        bpy.ops.object.select_all(action='SELECT')
        bpy.ops.object.delete()
        logger.info("✅ Scene cleared")
    
    def import_obj(self, filepath):
        """Import OBJ file"""
        try:
            bpy.ops.wm.obj_import(filepath=str(filepath))
            logger.info(f"✅ Imported: {filepath.name}")
            return True
        except Exception as e:
            logger.error(f"❌ Failed to import {filepath.name}: {e}")
            return False
    
    def import_fbx(self, filepath):
        """Import FBX file"""
        try:
            bpy.ops.import_scene.fbx(filepath=str(filepath))
            logger.info(f"✅ Imported: {filepath.name}")
            return True
        except Exception as e:
            logger.error(f"❌ Failed to import {filepath.name}: {e}")
            return False
    
    def export_gltf(self, filepath):
        """Export scene to glTF format"""
        try:
            bpy.ops.export_scene.gltf(
                filepath=str(filepath),
                export_format='GLB',
                export_texcoords=True,
                export_normals=True,
                export_materials='EXPORT',
                export_colors=True
            )
            logger.info(f"✅ Exported: {filepath.name}")
            return True
        except Exception as e:
            logger.error(f"❌ Failed to export {filepath.name}: {e}")
            return False
    
    def optimize_mesh(self):
        """Optimize mesh for game engine use"""
        try:
            # Select all mesh objects
            bpy.ops.object.select_all(action='DESELECT')
            for obj in bpy.context.scene.objects:
                if obj.type == 'MESH':
                    obj.select_set(True)
                    bpy.context.view_layer.objects.active = obj
            
            # Apply modifiers
            for obj in bpy.context.selected_objects:
                if obj.type == 'MESH':
                    for modifier in obj.modifiers:
                        bpy.ops.object.modifier_apply(modifier=modifier.name)
            
            # Triangulate
            bpy.ops.object.mode_set(mode='EDIT')
            bpy.ops.mesh.select_all(action='SELECT')
            bpy.ops.mesh.quads_convert_to_tris()
            bpy.ops.object.mode_set(mode='OBJECT')
            
            logger.info("✅ Mesh optimized")
            return True
        except Exception as e:
            logger.error(f"❌ Mesh optimization failed: {e}")
            return False
    
    def generate_lods(self, levels=3):
        """Generate Level of Detail (LOD) meshes"""
        try:
            for i in range(levels):
                ratio = 1.0 - (i + 1) * 0.25
                lod_name = f"LOD{i}"
                
                # Duplicate and decimate
                bpy.ops.object.duplicate()
                obj = bpy.context.active_object
                obj.name = lod_name
                
                # Add decimate modifier
                mod = obj.modifiers.new(name="Decimate", type='DECIMATE')
                mod.ratio = ratio
                bpy.ops.object.modifier_apply(modifier="Decimate")
                
                logger.info(f"✅ Generated {lod_name} at {ratio*100}% detail")
            
            return True
        except Exception as e:
            logger.error(f"❌ LOD generation failed: {e}")
            return False
    
    def batch_convert_2d_to_3d(self, image_dir, depth=0.1):
        """Convert 2D images to 3D planes with geometry"""
        try:
            image_files = list(Path(image_dir).glob("*.png")) + list(Path(image_dir).glob("*.jpg"))
            
            for img_path in image_files:
                self.clear_scene()
                
                # Create plane
                bpy.ops.mesh.primitive_plane_add(size=2)
                plane = bpy.context.active_object
                
                # Create material with texture
                mat = bpy.data.materials.new(name=f"Material_{img_path.stem}")
                mat.use_nodes = True
                nodes = mat.node_tree.nodes
                
                # Add image texture
                tex_node = nodes.new('ShaderNodeTexImage')
                tex_node.image = bpy.data.images.load(str(img_path))
                
                # Connect to material output
                bsdf = nodes.get('Principled BSDF')
                mat.node_tree.links.new(tex_node.outputs['Color'], bsdf.inputs['Base Color'])
                
                plane.data.materials.append(mat)
                
                # Add displacement for 3D effect
                mod = plane.modifiers.new(name="Displace", type='DISPLACE')
                mod.strength = depth
                
                # Export
                output_path = self.output_dir / f"{img_path.stem}_3d.glb"
                self.export_gltf(output_path)
                
                logger.info(f"✅ Converted 2D→3D: {img_path.name}")
            
            return True
        except Exception as e:
            logger.error(f"❌ 2D→3D conversion failed: {e}")
            return False
    
    def process_directory(self, input_dir=None, output_dir=None):
        """Process all 3D files in directory"""
        if input_dir:
            self.input_dir = Path(input_dir)
        if output_dir:
            self.output_dir = Path(output_dir)
        
        # Get all supported files
        supported_formats = ['*.obj', '*.fbx', '*.blend']
        files = []
        for fmt in supported_formats:
            files.extend(self.input_dir.glob(fmt))
        
        logger.info(f"📁 Found {len(files)} files to process")
        
        for filepath in files:
            try:
                self.clear_scene()
                
                # Import based on format
                if filepath.suffix == '.obj':
                    self.import_obj(filepath)
                elif filepath.suffix == '.fbx':
                    self.import_fbx(filepath)
                elif filepath.suffix == '.blend':
                    bpy.ops.wm.open_mainfile(filepath=str(filepath))
                
                # Optimize
                self.optimize_mesh()
                
                # Export to glTF
                output_path = self.output_dir / f"{filepath.stem}.glb"
                self.export_gltf(output_path)
                
                logger.info(f"✅ Processed: {filepath.name} → {output_path.name}")
                
            except Exception as e:
                logger.error(f"❌ Failed to process {filepath.name}: {e}")
                continue
        
        logger.info("🎉 Batch processing complete!")

def main():
    """Main execution function"""
    logger.info("🤖 MOTHER-BRAIN Blender Automation")
    logger.info("=" * 50)
    
    # Parse command line arguments
    import argparse
    parser = argparse.ArgumentParser(description='Blender automation for MOTHER-BRAIN')
    parser.add_argument('--input', '-i', default='assets/input', help='Input directory')
    parser.add_argument('--output', '-o', default='assets/output', help='Output directory')
    parser.add_argument('--mode', '-m', choices=['convert', '2d-to-3d', 'optimize'], 
                       default='convert', help='Operation mode')
    parser.add_argument('--image-dir', default='assets/2d', help='2D image directory for conversion')
    
    # Only parse known args to avoid Blender's own arguments
    args, unknown = parser.parse_known_args()
    
    automation = BlenderAutomation(input_dir=args.input, output_dir=args.output)
    
    if args.mode == 'convert':
        automation.process_directory()
    elif args.mode == '2d-to-3d':
        automation.batch_convert_2d_to_3d(args.image_dir)
    elif args.mode == 'optimize':
        automation.optimize_mesh()
        output_path = Path(args.output) / "optimized.glb"
        automation.export_gltf(output_path)
    
    logger.info("✅ Blender automation complete!")

if __name__ == "__main__":
    main()
