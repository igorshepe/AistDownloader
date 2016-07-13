// Decompiled with JetBrains decompiler
// Type: owp.FDownloader.FinamTreeView
// Assembly: owp.FDownloader, Version=2010.6.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 5BA469CD-6B26-4EBF-9E55-D0FECCDFAB62
// Assembly location: C:\Users\INVEST\Desktop\Котировки финам\finam data owp.FDownloader\owp.FDownloader.exe

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace owp.FDownloader
{
  public class FinamTreeView : TreeView
  {
    private IContainer components;
    private ImageList imageList;

    public FinamTreeView()
    {
      this.InitializeComponent();
      this.ImageList = this.imageList;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (FinamTreeView));
      this.imageList = new ImageList(this.components);
      this.SuspendLayout();
      this.imageList.ImageStream = (ImageListStreamer) componentResourceManager.GetObject("imageList.ImageStream");
      this.imageList.TransparentColor = Color.Transparent;
      this.imageList.Images.SetKeyName(0, "0.gif");
      this.imageList.Images.SetKeyName(1, "1.gif");
      this.imageList.Images.SetKeyName(2, "2.gif");
      this.LineColor = Color.Black;
      this.ResumeLayout(false);
    }

    private void ChangeNodeState(TreeNode node)
    {
      if (node == null)
        return;
      this.BeginUpdate();
      if ((node.Tag as EmitentInfo).id != -1)
      {
        node.ImageIndex = node.ImageIndex == 0 || node.ImageIndex < 0 ? 1 : 0;
        this.ChangeNodeState(node.Parent);
      }
      else
      {
        bool flag1 = false;
        bool flag2 = true;
        foreach (TreeNode node1 in node.Nodes)
        {
          flag1 |= node1.ImageIndex == 1;
          flag2 &= node1.ImageIndex == 1;
        }
        node.ImageIndex = !flag2 ? (!flag1 ? 0 : 2) : 1;
      }
      node.SelectedImageIndex = node.ImageIndex;
      this.EndUpdate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      this.ChangeNodeState(this.GetNodeAt(this.PointToClient(Control.MousePosition)));
      base.OnMouseDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (e.KeyCode != Keys.Space)
        return;
      this.ChangeNodeState(this.SelectedNode);
    }

    public List<EmitentInfo> GetEmitents()
    {
      List<EmitentInfo> emitentInfoList = new List<EmitentInfo>();
      foreach (TreeNode node1 in this.Nodes)
      {
        foreach (TreeNode node2 in node1.Nodes)
        {
          (node2.Tag as EmitentInfo).checed = node2.ImageIndex == 1;
          emitentInfoList.Add(node2.Tag as EmitentInfo);
        }
      }
      return emitentInfoList;
    }

    private void AddEmitent(TreeNode node, EmitentInfo emitent)
    {
      TreeNode treeNode = node.Nodes.Add(emitent.name);
      treeNode.Tag = (object) emitent;
      if (!emitent.checed)
        return;
      treeNode.ImageIndex = 1;
      treeNode.SelectedImageIndex = 1;
    }

    public void SetEmitents(List<EmitentInfo> emitents)
    {
      this.Nodes.Clear();
      this.BeginUpdate();
      TreeNode node1 = (TreeNode) null;
      foreach (EmitentInfo emitent in emitents)
      {
        if (node1 != null && (node1.Tag as EmitentInfo).marketId == emitent.marketId)
        {
          this.AddEmitent(node1, emitent);
        }
        else
        {
          node1 = (TreeNode) null;
          foreach (TreeNode node2 in this.Nodes)
          {
            if ((node2.Tag as EmitentInfo).marketId == emitent.marketId)
            {
              node1 = node2;
              this.AddEmitent(node1, emitent);
              break;
            }
          }
          if (node1 == null)
          {
            node1 = this.Nodes.Add(emitent.marketName);
            node1.Tag = (object) new EmitentInfo(emitent.marketId, emitent.marketName, -1, string.Empty, string.Empty);
            this.AddEmitent(node1, emitent);
          }
        }
      }
      foreach (TreeNode node2 in this.Nodes)
        this.ChangeNodeState(node2);
      this.EndUpdate();
    }
  }
}
