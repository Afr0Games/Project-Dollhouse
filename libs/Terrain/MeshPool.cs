using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Graphics;

namespace Hoodtest
{
    /// <summary>
    /// A threadsafe pool for meshes.
    /// </summary>
    public class MeshPool
    {
        private ConcurrentStack<CityMesh> m_Pool = new ConcurrentStack<CityMesh>();
        private GraphicsDevice graphicsDevice;
        private Effect m_Effect;

        /// <summary>
        /// Constructs a new MeshPool instance.
        /// </summary>
        /// <param name="graphicsDevice">An <see cref="GraphicsDevice"/> instance.</param>
        /// <param name="effect">An <see cref="Effect"/> instance.</param>
        public MeshPool(GraphicsDevice graphicsDevice, Effect effect)
        {
            this.graphicsDevice = graphicsDevice;
            this.m_Effect = effect;
        }

        /// <summary>
        /// Gets a mesh from this pool.
        /// </summary>
        /// <returns>The mesh.</returns>
        public CityMesh GetMesh()
        {
            CityMesh PotentialMesh;
            if (m_Pool.TryPop(out PotentialMesh))
            {
                PotentialMesh.Reset();
                return PotentialMesh;
            }
            else
                return new CityMesh(graphicsDevice, m_Effect);
        }

        public void ReturnMesh(CityMesh Mesh)
        {
            Mesh.Clear();
            m_Pool.Push(Mesh);
        }
    }
}
