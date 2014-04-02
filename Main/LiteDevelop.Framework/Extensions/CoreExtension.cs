using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LiteDevelop.Framework.Mui;

namespace LiteDevelop.Framework.Extensions
{
    public sealed class CoreExtension : LiteExtension
    {
        public static CoreExtension Instance { get; private set; }
        private MuiProcessor _muiProcessor;

        public CoreExtension()
        {
            if (Instance != null)
                throw new InvalidOperationException("Cannot create a second instance of CoreExtension");

            Instance = this;
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return "LiteDevelop Framework"; }
        }

        /// <inheritdoc />
        public override string Description
        {
            get { return "LiteDevelop Core Framework"; }
        }

        /// <inheritdoc />
        public override string Author
        {
            get { return "Jerre S."; }
        }

        /// <inheritdoc />
        public override Version Version
        {
            get { return typeof(CoreExtension).Assembly.GetName().Version; }
        }

        /// <inheritdoc />
        public override string Copyright
        {
            get { return "Copyright © Jerre S. 2014"; }
        }

        /// <inheritdoc />
        public override void Initialize(InitializationContext context)
        {
            _muiProcessor = new MuiProcessor(context.Host, Path.Combine(Application.StartupPath, "MUI"));
        }

        public MuiProcessor MuiProcessor
        {
            get { return _muiProcessor; }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            Instance = null;
        }

    }
}
