﻿using AttachedCommandBehavior;
using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Helpers;
using LedMusic2.Models;
using LedMusic2.Nodes;
using LedMusic2.Sound;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace LedMusic2.ViewModels
{
    public class MainViewModel : VMBase
    {

        #region Singleton and Constructor
        private static MainViewModel _instance = new MainViewModel();
        public static MainViewModel Instance
        {
            get { return _instance; }
        }
        private MainViewModel() {

            PropertyChanged += MainViewModel_PropertyChanged;
            SoundEngine.Instance.PropertyChanged += SoundEngine_PropertyChanged;
            GlobalProperties.Instance.PropertyChanged += GlobalProperties_PropertyChanged;

            CmdPlayPause = new SimpleCommand();
            CmdPlayPause.ExecuteDelegate = (o) => {
                if (SoundEngine.Instance.CanPlay)
                    SoundEngine.Instance.Play();
                else
                    SoundEngine.Instance.Pause();
            };
            CmdPlayPause.CanExecuteDelegate = (o) => SoundEngine.Instance.CanPlay || SoundEngine.Instance.CanPause;

            CmdStop = new SimpleCommand();
            CmdStop.ExecuteDelegate = (o) => SoundEngine.Instance.Stop();
            CmdStop.CanExecuteDelegate = (o) => SoundEngine.Instance.CanStop;

            CmdOpenMusic = new SimpleCommand();
            CmdOpenMusic.ExecuteDelegate = (o) => openMusic();

            CmdSaveProject = new SimpleCommand();
            CmdSaveProject.ExecuteDelegate = (o) => save();

            CmdOpenProject = new SimpleCommand();
            CmdOpenProject.ExecuteDelegate = (o) => load();

            var c1 = new LedColorRGB(250, 250, 0);
            var c2 = c1.GetColorHSV();
            var c3 = c2.GetColorRGB();

        }
        #endregion

        #region Properties
        #region NodeDisplay
        private double _scale = 1.0;
        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value > 0.3)
                _scale = value;
                NotifyPropertyChanged();
            }
        }

        private double _scaleCenterX = 0.0;
        public double ScaleCenterX
        {
            get { return _scaleCenterX; }
            set
            {
                _scaleCenterX = value;
                NotifyPropertyChanged();
            }
        }

        private double _scaleCenterY = 0.0;
        public double ScaleCenterY
        {
            get { return _scaleCenterY; }
            set
            {
                _scaleCenterY = value;
                NotifyPropertyChanged();
            }
        }

        private double _translateX = 0.0;
        public double TranslateX
        {
            get { return _translateX; }
            set
            {
                _translateX = value;
                NotifyPropertyChanged();
            }
        }

        private double _translateY = 0.0;
        public double TranslateY
        {
            get { return _translateY; }
            set
            {
                _translateY = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<NodeBase> _nodes = new ObservableCollection<NodeBase>();
        public ObservableCollection<NodeBase> Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Connection> _connections = new ObservableCollection<Connection>();
        public ObservableCollection<Connection> Connections
        {
            get { return _connections; }
            set
            {
                _connections = value;
                NotifyPropertyChanged();
            }
        }

        private TemporaryConnectionViewModel _temporaryConnection;
        public TemporaryConnectionViewModel TemporaryConnection
        {
            get { return _temporaryConnection; }
            set
            {
                _temporaryConnection = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TemporaryConnectionOC");
            }
        }

        public ObservableCollection<TemporaryConnectionViewModel> TemporaryConnectionOC
        {
            get { return new ObservableCollection<TemporaryConnectionViewModel>() { TemporaryConnection }; }
        }

        private double _mousePosX = 0;
        public double MousePosX
        {
            get { return _mousePosX; }
            set
            {
                _mousePosX = value;
                NotifyPropertyChanged();
            }
        }

        private double _mousePosY = 0;
        public double MousePosY
        {
            get { return _mousePosY; }
            set
            {
                _mousePosY = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isAddNodePanelOpen = false;
        public bool IsAddNodePanelOpen
        {
            get { return _isAddNodePanelOpen; }
            set
            {
                _isAddNodePanelOpen = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<NodeCategoryModel> _nodeCategories = new ObservableCollection<NodeCategoryModel>();
        public ObservableCollection<NodeCategoryModel> NodeCategories
        {
            get { return _nodeCategories; }
            set
            {
                _nodeCategories = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Progress
        private ObservableCollection<ProgressViewModel> _progresses = new ObservableCollection<ProgressViewModel>();
        public ObservableCollection<ProgressViewModel> Progresses
        {
            get { return _progresses; }
            set
            {
                _progresses = value;
                NotifyPropertyChanged();
            }
        }

        public string ProgressText
        {
            get
            {
                if (_progresses.Count == 0)
                    return "";
                else if (_progresses.Count == 1)
                    return _progresses[0].Name;
                else
                    return _progresses.Count + " operations running...";
            }
        }

        public int ProgressValue
        {
            get
            {
                if (_progresses.Count == 0)
                    return 0;
                else if (_progresses.Count == 1)
                    return _progresses[0].Progress;
                else
                    return calcTotalProgressValue();
            }
        }
        #endregion

        private int _currentFrame = 0;
        public int CurrentFrame
        {
            get { return _currentFrame; }
            private set
            {
                _currentFrame = value;
                NotifyPropertyChanged();
            }
        }

        private double _playerPosition = 0; //IN PIXELS!!!
        public double PlayerPosition
        {
            get { return _playerPosition; }
            set
            {
                _playerPosition = value;
                NotifyPropertyChanged();
            }
        }

        private double _trackWidth = 1000;
        public double TrackWidth
        {
            get { return _trackWidth; }
            set
            {
                _trackWidth = value;
                NotifyPropertyChanged();
            }
        }

        private string _infotext = "";
        public string Infotext
        {
            get { return _infotext; }
            set
            {
                _infotext = value;
                NotifyPropertyChanged();
            }
        }

        #region Commands
        public SimpleCommand CmdOpenMusic { get; private set; }
        public SimpleCommand CmdPlayPause { get; private set; }
        public SimpleCommand CmdStop { get; private set; }
        public SimpleCommand CmdSaveProject { get; private set; }
        public SimpleCommand CmdOpenProject { get; private set; }
        #endregion
        #endregion

        #region Private Fields
        private NodeBase[] currentNodeCalculationOrder = new NodeBase[0];
        private NodeTreeBuilder ntb = new NodeTreeBuilder();
        private bool calculating = false;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            NodeBase.UnselectAllNodes += NodeBase_UnselectAllNodes;
            NodeBase.OutputChanged += NodeBase_OutputChanged;

            fillNodeCategories();
            calculateNodeTree();
        }

        public void End()
        {
            SoundEngine.Instance.Stop();
            SoundEngine.Instance.CleanupPlayback();

            foreach (Connection c in Connections)
                c.Dispose();

            Connections.Clear();

        }

        public void SetCurrentFrame(int n)
        {
            SoundEngine.Instance.Position = TimeSpan.FromSeconds((n + 0.5) / GlobalProperties.Instance.FPS);
        }

        #region Connections
        public void CreateTemporaryConnection(NodeInterface sender)
        {

            if (sender.IsInput && Connections.FirstOrDefault((x) => x.Output == sender) != null)
            {
                //Since the sender interface is an input and there already exists a connection, remove the old
                //connection and make it temporary so the user can either remove it or rewire it.
                var oldConnection = Connections.FirstOrDefault((x) => x.Output == sender);
                var input = oldConnection.Input;
                Connections.Remove(oldConnection);
                oldConnection.Dispose();
                TemporaryConnection = new TemporaryConnectionViewModel(input);
                NodeConnectionHelper.Instance.ConnectionOrigin = input;
            } else
            {
                TemporaryConnection = new TemporaryConnectionViewModel(sender);
                NodeConnectionHelper.Instance.ConnectionOrigin = sender;
            }                      

        }        

        public void MouseOverInterface(NodeInterface sender)
        {
            if (TemporaryConnection == null)
                return;

            TemporaryConnection.SetTargetInterface(sender);
            TemporaryConnection.IsConnectionAllowed = NodeConnectionHelper.Instance.CanConnect(sender);
            
        }

        public void MouseLeftInterface()
        {
            if (TemporaryConnection == null)
                return;

            TemporaryConnection.SetTargetInterface(null);
            TemporaryConnection.IsConnectionAllowed = false;
        }

        public void FinalizeTemporaryConnection()
        {

            if (TemporaryConnection.TargetInterface != null && TemporaryConnection.IsConnectionAllowed)
            {

                NodeInterface input, output;
                if (TemporaryConnection.OriginInterface.IsInput)
                {
                    //Input & Output switched in connection, because for the connection
                    //the output of a node is actually the input and vice versa
                    output = TemporaryConnection.OriginInterface;
                    input = TemporaryConnection.TargetInterface;
                } else
                {
                    input = TemporaryConnection.OriginInterface;
                    output = TemporaryConnection.TargetInterface;
                }

                //Check if the output of the connection aka the input of a node already
                //has a connection to it and if so, remove it, because only one connection
                //per input is allowed.
                var oldC = Connections.FirstOrDefault((x) => x.Output == output);
                if (oldC != null)
                {
                    Connections.Remove(oldC);
                    oldC.Dispose();
                }

                var c = new Connection(input, output);
                Connections.Add(c);

                calculateNodeTree();
                CalculateNodes(input.Parent);

            }

            TemporaryConnection.Dispose();
            TemporaryConnection = null;
            NodeConnectionHelper.Instance.ConnectionOrigin = null;

        }

        public void RecalculateAllConnectionPoints()
        {
            foreach (var c in Connections)
                c.CalculatePoints();
        }
        #endregion

        #region Nodes
        public void CalculateAllNodes()
        {
            CalculateNodes(0);
        }

        public void CalculateNodes(NodeBase startingNode)
        {
            var i = Array.IndexOf(currentNodeCalculationOrder, startingNode);
            if (i >= 0 && i < currentNodeCalculationOrder.Length)
                CalculateNodes(i);
        }

        public void CalculateNodes(int startingIndex)
        {
            calculating = true;
            NodeBase.FireOutputChangedEvents = false;
            for (int i = startingIndex; i < currentNodeCalculationOrder.Length; i++)
            {
                currentNodeCalculationOrder[i].Calculate();
            }
            calculating = false;
            NodeBase.FireOutputChangedEvents = true;
        }

        public void DeleteSelectedNode()
        {
            DeleteNode(Nodes.FirstOrDefault((x) => x.IsSelected));
        }

        public void DeleteNode(NodeBase node)
        {

            if (node == null)
                return;

            //Delete all the connections from and to the node.
            var toDelete = Connections.Where((x) => x.Input.Parent == node || x.Output.Parent == node).ToArray();

            foreach (Connection c in toDelete)
            {
                Connections.Remove(c);
                c.Dispose();
            }

            Nodes.Remove(node);

        }

        public NodeBase AddNode(NodeType t)
        {

            if (t == null)
                return null;

            var constructor = t.T.GetConstructor(new Type[] { typeof(Point) });
            var node = (NodeBase)constructor.Invoke(new object[] { new Point(MousePosX - TranslateX, MousePosY - TranslateY) });
            Nodes.Add(node);
            return node;

        }
        #endregion

        #region Progress
        public void AddProgress(ProgressViewModel vm)
        {
            vm.PropertyChanged += Progress_PropertyChanged;
            _progresses.Add(vm);
            NotifyPropertyChanged("ProgressText");
            NotifyPropertyChanged("ProgressValue");
        }

        public void RemoveProgress(ProgressViewModel vm)
        {
            _progresses.Remove(vm);
            vm.PropertyChanged -= Progress_PropertyChanged;
            NotifyPropertyChanged("ProgressText");
            NotifyPropertyChanged("ProgressValue");
        }
        #endregion
        #endregion

        #region PropertyChangedEvents
        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TrackWidth")
            {
                updatePlayerPosition();
            }
            else if (e.PropertyName == "CurrentFrame")
            {
                updatePlayerPosition();
                CalculateAllNodes();
            }
        }

        private void SoundEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Position")
                calculateCurrentFrame();
            else if (e.PropertyName == "Length")
                updatePlayerPosition();
        }

        private void GlobalProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FPS")
                calculateCurrentFrame();
        }

        private void Progress_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("ProgressText");
            NotifyPropertyChanged("ProgressValue");
        }
        #endregion

        #region Private Methods
        private void NodeBase_UnselectAllNodes(object sender, EventArgs e)
        {
            foreach (NodeBase n in Nodes)
                n.IsSelected = false;
        }

        private void NodeBase_OutputChanged(object sender, EventArgs e)
        {
            if (!calculating && sender is NodeBase)
                CalculateNodes((NodeBase)sender);
        }

        private void fillNodeCategories()
        {

            //Get all node classes
            var nodeClasses = Assembly.GetCallingAssembly().GetTypes().Where((t) => string.Equals(t.Namespace, "LedMusic2.Nodes") && !t.IsAbstract);
            foreach (NodeCategory category in Enum.GetValues(typeof(NodeCategory)))
            {
                var c = new NodeCategoryModel(Enum.GetName(typeof(NodeCategory), category));
                var n = nodeClasses.Where((t) => {
                        var x = t.GetCustomAttribute(typeof(NodeAttribute));
                        if (x == null)
                            return false;
                        return ((NodeAttribute)x).Category == category;
                    });

                foreach (var x in n)
                    c.NodeTypes.Add(new NodeType(((NodeAttribute)x.GetCustomAttribute(typeof(NodeAttribute))).Name, x));

                NodeCategories.Add(c);
            }

        }

        private void calculateNodeTree()
        {
            var nodes = Nodes.ToArray();
            var order = ntb.GetCalculationOrder(ntb.GetRootElements(nodes), nodes, Connections.ToArray());
            if (order == null)
                MessageBox.Show("Failed to calculate node tree.");
            else
                currentNodeCalculationOrder = order;
        }

        private int calcTotalProgressValue()
        {
            var val = 0;
            foreach (var x in _progresses)
            {
                val += x.Progress;
            }
            return (int)Math.Floor((double)val / _progresses.Count);
        }

        private void updatePlayerPosition()
        {

            double trackDuration = SoundEngine.Instance.Length.TotalSeconds;
            if (trackDuration == 0)
                return;

            double secondWidth = (TrackWidth / trackDuration);
            PlayerPosition = ((double)CurrentFrame / GlobalProperties.Instance.FPS) * secondWidth;

        }

        private void calculateCurrentFrame()
        {
            int fps = GlobalProperties.Instance.FPS;
            double time = SoundEngine.Instance.Position.TotalSeconds;
            CurrentFrame = (int)Math.Floor(time * fps);
        }

        private async void openMusic()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.Filter = "Audio files (*.wav;*.mp3)|*.wav;*mp3";
            if (ofd.ShowDialog() == true)
            {
                await SoundEngine.Instance.OpenFile(ofd.FileName);
            }
        }
        #endregion

        #region Saving and Loading
        private void save()
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = "lmp";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (sfd.ShowDialog() != true)
            {
                return;
            }

            Stream s = sfd.OpenFile();

            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", "UTF-8", "yes");

            XElement rootX = new XElement("ledmusicproject");

            XElement translateX = new XElement("translate");
            translateX.SetAttributeValue("x", TranslateX.ToString(CultureInfo.InvariantCulture));
            translateX.SetAttributeValue("y", TranslateY.ToString(CultureInfo.InvariantCulture));

            XElement currentFrameX = new XElement("currentframe", CurrentFrame);
            XElement musicFileX = new XElement("musicfile", SoundEngine.Instance.File);

            rootX.Add(translateX, currentFrameX, musicFileX);

            XElement nodesX = new XElement("nodes");
            foreach (NodeBase n in Nodes)
            {
                nodesX.Add(n.GetXmlElement());
            }
            rootX.Add(nodesX);

            XElement connectionsX = new XElement("connections");
            foreach (Connection c in Connections)
            {
                connectionsX.Add(c.GetXmlElement());
            }
            rootX.Add(connectionsX);

            doc.Add(rootX);
            doc.Save(s);
            s.Close();

        }

        private void load()
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LedMusic Project File|*.lmp";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() != true)
                return;

            ProgressViewModel prg = new ProgressViewModel("Loading project");
            AddProgress(prg);

            Stream s = ofd.OpenFile();

            try
            {

                XDocument doc = XDocument.Load(s);

                foreach (Connection c in Connections)
                {
                    c.Dispose();
                }
                Connections.Clear();
                Nodes.Clear();

                XElement rootX = (XElement)doc.FirstNode;
                int totalElements = rootX.Elements().Count();
                int counter = 0;

                foreach (XElement n in rootX.Elements())
                {
                    switch (n.Name.LocalName)
                    {

                        case "translate":
                            TranslateX = double.Parse(n.Attribute("x").Value, CultureInfo.InvariantCulture);
                            TranslateY = double.Parse(n.Attribute("y").Value, CultureInfo.InvariantCulture);
                            break;

                        case "currentframe":
                            CurrentFrame = int.Parse(n.Value);
                            break;

                        case "musicfile":
                            if (File.Exists(n.Value))
                                Task.Run(() => SoundEngine.Instance.OpenFile(n.Value));
                            else
                                MessageBox.Show("Couldn't locate music file. Please open it manually.");
                            break;

                        case "nodes":
                            foreach (XElement nodeX in n.Elements())
                                loadNode(nodeX);
                            break;

                        case "connections":
                            foreach (XElement connectionX in n.Elements())
                                loadConnection(connectionX);
                            break;

                    }

                    counter++;
                    prg.Progress = (int)(1.0 * counter / totalElements) * 100;

                }

                calculateNodeTree();
                CalculateAllNodes();
                //TODO: make connections draw when the nodes are displayed
                

            } catch (XmlException e)
            {
                MessageBox.Show("Failed to open project file: " + e.Message);
                Console.WriteLine(e.StackTrace);
            } finally
            {
                s.Close();
                RemoveProgress(prg);
            }

        }

        private void loadNode(XElement nodeX)
        {

            string type = nodeX.Attribute("type").Value;
            NodeBase nodeInstance = null;
            foreach (NodeCategoryModel ncm in NodeCategories)
            {
                foreach (NodeType n in ncm.NodeTypes)
                {
                    if (n.Name == type)
                    {
                        nodeInstance = AddNode(n);
                        break;
                    }
                }
                if (nodeInstance != null)
                    break;
            }

            nodeInstance.LoadFromXml(nodeX);

        }

        private void loadConnection(XElement connectionX)
        {

            Guid inputId = Guid.Empty, outputId = Guid.Empty;

            foreach (XElement el in connectionX.Elements())
            {
                if (el.Name.LocalName == "input")
                    inputId = Guid.Parse(el.Attribute("interfaceid").Value);
                else if (el.Name.LocalName == "output")
                    outputId = Guid.Parse(el.Attribute("interfaceid").Value);
            }

            if (inputId == Guid.Empty || outputId == Guid.Empty)
                return;

            NodeInterface input = findInterfaceById(inputId);
            NodeInterface output = findInterfaceById(outputId);

            if (input == null || output == null)
                return;

            Connection c = new Connection(input, output);
            Connections.Add(c);

        }

        private NodeInterface findInterfaceById(Guid id)
        {
            foreach (NodeBase n in Nodes)
            {
                foreach (NodeInterface ni in n.Inputs)
                {
                    if (ni.Id == id)
                        return ni;
                }
                foreach (NodeInterface ni in n.Outputs)
                {
                    if (ni.Id == id)
                        return ni;
                }
            }
            return null;
        }
        #endregion

    }
}
