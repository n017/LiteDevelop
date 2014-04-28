using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Essentials.CodeEditor.Gui.Styles
{
    public class StyleMap
    {
        private AppearanceMap _map, _fallBackMap;

        public StyleMap(AppearanceMap map, AppearanceMap fallBackMap)
        {
            _map = map;
            _fallBackMap = fallBackMap;

            DefaultText = new DynamicTextStyle(GetDescription("DefaultText"));
            KeywordStyle = new DynamicTextStyle(GetDescription("Keyword"));
            StringStyle = new DynamicTextStyle(GetDescription("String"));
            NumberStyle = new DynamicTextStyle(GetDescription("Number"));
            TypeDefinitionStyle = new DynamicTextStyle(GetDescription("TypeDefinition"));
            CommentStyle = new DynamicTextStyle(GetDescription("Comment"));
            LineNumbers = new DynamicTextStyle(GetDescription("LineNumbers"));
            AttributeStyle = new DynamicTextStyle(GetDescription("Attribute"));
            BookmarkStyle = new DynamicTextStyle(GetDescription("Bookmark"));
            BreakpointStyle = new DynamicTextStyle(GetDescription("Breakpoint"));
            InstructionPointerStyle = new DynamicTextStyle(GetDescription("InstructionPointer"));
            ShadowInstructionPointerStyle = new DynamicTextStyle(GetDescription("ShadowInstructionPointer"));
            CurrentLineStyle = new DynamicTextStyle(GetDescription("CurrentLine"));
            ChangedLineStyle = new DynamicTextStyle(GetDescription("ChangedLine"));
            SelectionStyle = new DynamicTextStyle(GetDescription("Selection"));
        }

        private AppearanceDescription GetDescription(string id)
        {
            AppearanceDescription description = _map.GetDescriptionById(id);
            if (description == null && (description = _fallBackMap.GetDescriptionById(id)) != null)
            {
                _map.Descriptions.Add(description);
            }
            return description;
        }

        public DynamicTextStyle DefaultText
        {
            get;
            private set;
        }

        public DynamicTextStyle KeywordStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle StringStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle TypeDefinitionStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle NumberStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle CommentStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle LineNumbers
        {
            get;
            private set;
        }

        public DynamicTextStyle AttributeStyle
        {
            get;
            private set;
        }

        public FastColoredTextBoxNS.TextStyle BookmarkStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle BreakpointStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle InstructionPointerStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle ShadowInstructionPointerStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle CurrentLineStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle ChangedLineStyle
        {
            get;
            private set;
        }

        public DynamicTextStyle SelectionStyle
        {
            get;
            private set;
        }
    }
}
