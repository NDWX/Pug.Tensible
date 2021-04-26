using Settings;
using Settings.Schema;

namespace Pug.Tensible.Tests
{
	public class TestContext
	{
		public IUnifier Unifier { get; set; } = new Unifier(collection => { });

		public IUnified Unified { get; set; }
	}
}