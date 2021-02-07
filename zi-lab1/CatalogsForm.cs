using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zi_lab1
{
    public partial class CatalogsForm : Form
    {
        private readonly Dictionary<TreeNode, Entity> linkedEntities = new Dictionary<TreeNode, Entity>();

        public CatalogsForm()
        {
            InitializeComponent();
            
        }

        private void CatalogsForm_Load(object sender, EventArgs e)
        {
            foreach (var entity in Context.Entities
                .Where(e => e.Path.Equals(string.Empty) && Context.CurrentUser.PermissionLevel >= e.PermissionLevel))
            {
                if (entity is File file && !file.AccessRights.CanRead())
                    return;

                var node = new TreeNode(entity.Name);
                
                if(!linkedEntities.ContainsKey(node))
                    linkedEntities.Add(node, entity);

                FillTreeNode(node);
                treeView1.Nodes.Add(node);
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            var entity = linkedEntities[e.Node];

            var fullpath = $"{entity.Path}/{entity.Name}";
            foreach(var ent in Context.Entities
                .Where(e => e.Path.Equals(fullpath) && Context.CurrentUser.PermissionLevel >= e.PermissionLevel))
            {
                if (ent is File file && !file.AccessRights.CanRead())
                    return;

                var node = new TreeNode(ent.Name);
                if (!linkedEntities.ContainsKey(node))
                    linkedEntities.Add(node, ent);

                FillTreeNode(node);
                e.Node.Nodes.Add(node);
            }
        }

        void FillTreeNode(TreeNode treeNode)
        {
            var entity = linkedEntities[treeNode];

            var fullpath = $"{entity.Path}/{entity.Name}";
            foreach (var ent in Context.Entities
                .Where(e => e.Path.Equals(fullpath) && Context.CurrentUser.PermissionLevel >= e.PermissionLevel))
            {
                if (ent is File file && !file.AccessRights.CanRead())
                    return;

                var node = new TreeNode(ent.Name);
                if (!linkedEntities.ContainsKey(node))
                    linkedEntities.Add(node, ent);
                treeNode.Nodes.Add(node);
            }
        }


        private void CreateButton_Click(object sender, EventArgs e)
        {
            var entity = linkedEntities[treeView1.SelectedNode];
            if (entity is File)
                entity = linkedEntities[treeView1.SelectedNode.Parent];
            MessageBox.Show(entity.Name);


            var accessRights = new Dictionary<User, List<Permission>>()
            {
                { Context.CurrentUser, new List<Permission>() { Permission.Read, Permission.Write} }
            };
            var newEntity = new File("defaultName", $"{entity.Path}/{entity.Name}", Context.CurrentUser.PermissionLevel,
                accessRights);
            Context.Entities.Add(newEntity);
            var node = entity is File ? treeView1.SelectedNode.Parent : treeView1.SelectedNode;
            node.Nodes.Clear();
            FillTreeNode(node);
        }


        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var entity = linkedEntities[e.Node];

            if (entity is File)
            {
                MessageBox.Show($"Файл {entity.Name} открыт (ну типа)");
            }
        }
    }
}
