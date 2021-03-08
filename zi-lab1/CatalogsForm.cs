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

        private File bufferedEntity = null;

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

                if (!linkedEntities.ContainsKey(node))
                {
                    linkedEntities.Add(node, entity);

                    FillTreeNode(node);
                    treeView1.Nodes.Add(node);
                }
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
                {
                    linkedEntities.Add(node, ent);

                    FillTreeNode(node);
                    e.Node.Nodes.Add(node);
                }
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
                {
                    linkedEntities.Add(node, ent);
                    FillTreeNode(node);
                    treeNode.Nodes.Add(node);
                }
            }
        }


        private void CreateButton_Click(object sender, EventArgs e)
        {

            var entity = linkedEntities[treeView1.SelectedNode];
            if (entity is File)
                entity = linkedEntities[treeView1.SelectedNode.Parent];

            if (entity.PermissionLevel < Context.CurrentUser.PermissionLevel)
            {
                MessageBox.Show("Недостаточно прав!");
                return;
            }

            var form = new CreateFileForm();
            form.ShowDialog();

            var accessRights = new Dictionary<User, List<Permission>>();
            foreach(var pair in form.AccessMatrixTextButton.Text.Split(";"))
            {
                var data = pair.Split(":");
                var login = data[0];
                var rights = data[1];

                var permissions = new List<Permission>();
                if (rights.Contains("r"))
                    permissions.Add(Permission.Read);
                if (rights.Contains("w"))
                    permissions.Add(Permission.Write);

                var user = Context.Users.Single(u => u.Username.Equals(login));

                accessRights.Add(user, permissions);
            }

            Entity newEntity;
            if (form.radioButton1.Checked)
                newEntity = new File(form.FilenameTextBox.Text, $"{entity.Path}/{entity.Name}",
                    Context.CurrentUser.PermissionLevel, accessRights);
            else
                newEntity = new Folder(form.FilenameTextBox.Text, $"{entity.Path}/{entity.Name}",
                    Context.CurrentUser.PermissionLevel);
            Context.Entities.Add(newEntity);


            var node = entity is File ? treeView1.SelectedNode.Parent : treeView1.SelectedNode;
            node.Nodes.Clear();
            FillTreeNode(node);
            using (var sw = System.IO.File.AppendText(@"C:\Users\grego\Desktop\zi-lab1\zi-lab1\zi-lab1\data.txt"))
            {
                sw.WriteLine(newEntity.ToString());
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var entity = linkedEntities[treeView1.SelectedNode];

            if (entity.PermissionLevel < Context.CurrentUser.PermissionLevel)
            {
                MessageBox.Show("Недостаточно прав!");
                return;
            }

            if(entity is File file && !file.AccessRights.CanWrite())
            {
                MessageBox.Show("Недостаточно прав!");
                return;
            }

            var node = treeView1.SelectedNode;
            var parentNode = node.Parent;
            node.Remove();
            Context.Entities.Remove(entity);
            parentNode.Nodes.Clear();
            FillTreeNode(parentNode);

            var lines = System.IO.File.ReadAllLines(@"C:\Users\grego\Desktop\zi-lab1\zi-lab1\zi-lab1\data.txt");
            System.IO.File.WriteAllLines(@"C:\Users\grego\Desktop\zi-lab1\zi-lab1\zi-lab1\data.txt",
                lines.Where(l => l.Split(",")[1] != entity.Guid.ToString()));
        }


        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var entity = linkedEntities[e.Node];

            if (entity is File)
            {
                MessageBox.Show($"Файл {entity.Name} открыт");
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if(this.bufferedEntity != null)
            {

                var entity = linkedEntities[treeView1.SelectedNode];
                if (entity is File)
                    entity = linkedEntities[treeView1.SelectedNode.Parent];

                if(entity.PermissionLevel < bufferedEntity.PermissionLevel)
                {
                    MessageBox.Show("Недостаточно прав!");
                    bufferedEntity = null;
                    return;
                }

                var copiedEntity = new File(bufferedEntity.Name, $"{entity.Path}/{entity.Name}",
                    bufferedEntity.PermissionLevel, bufferedEntity.AccessRights);
                Context.Entities.Add(copiedEntity);
                using (var sw = System.IO.File.AppendText(@"C:\Users\grego\Desktop\zi-lab1\zi-lab1\zi-lab1\data.txt"))
                {
                    sw.WriteLine(copiedEntity.ToString());
                }

                var node = entity is File ? treeView1.SelectedNode.Parent : treeView1.SelectedNode;
                node.Nodes.Clear();
                FillTreeNode(node);
                bufferedEntity = null;
            }
            else
            {
                var entity = linkedEntities[treeView1.SelectedNode];

                if(entity is Folder)
                {
                    MessageBox.Show("Можно копировать только файлы!");
                }
                this.bufferedEntity = entity as File;
            }
        }
    }
}
