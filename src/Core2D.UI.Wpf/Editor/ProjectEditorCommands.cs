﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.UI.Wpf.Editor
{
    /// <summary>
    /// Project editor commands.
    /// </summary>
    public static class ProjectEditorCommands
    {
        /// <summary>
        /// Gets or sets New command.
        /// </summary>
        public static ICommand NewCommand { get; set; }

        /// <summary>
        /// Gets or sets Open command.
        /// </summary>
        public static ICommand OpenCommand { get; set; }

        /// <summary>
        /// Gets or sets Close command.
        /// </summary>
        public static ICommand CloseCommand { get; set; }

        /// <summary>
        /// Gets or sets Save command.
        /// </summary>
        public static ICommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets SaveAs command.
        /// </summary>
        public static ICommand SaveAsCommand { get; set; }

        /// <summary>
        /// Gets or sets ExecuteScript command.
        /// </summary>
        public static ICommand ExecuteScriptCommand { get; set; }

        /// <summary>
        /// Gets or sets ImportObject command.
        /// </summary>
        public static ICommand ImportObjectCommand { get; set; }

        /// <summary>
        /// Gets or sets ExportObject command.
        /// </summary>
        public static ICommand ExportObjectCommand { get; set; }

        /// <summary>
        /// Gets or sets ImportXaml command.
        /// </summary>
        public static ICommand ImportXamlCommand { get; set; }

        /// <summary>
        /// Gets or sets ExportXaml command.
        /// </summary>
        public static ICommand ExportXamlCommand { get; set; }

        /// <summary>
        /// Gets or sets ImportJson command.
        /// </summary>
        public static ICommand ImportJsonCommand { get; set; }

        /// <summary>
        /// Gets or sets ExportJson command.
        /// </summary>
        public static ICommand ExportJsonCommand { get; set; }

        /// <summary>
        /// Gets or sets Export command.
        /// </summary>
        public static ICommand ExportCommand { get; set; }

        /// <summary>
        /// Gets or sets Exit command.
        /// </summary>
        public static ICommand ExitCommand { get; set; }

        /// <summary>
        /// Gets or sets ImportData command.
        /// </summary>
        public static ICommand ImportDataCommand { get; set; }

        /// <summary>
        /// Gets or sets ExportData command.
        /// </summary>
        public static ICommand ExportDataCommand { get; set; }

        /// <summary>
        /// Gets or sets UpdateData command.
        /// </summary>
        public static ICommand UpdateDataCommand { get; set; }

        /// <summary>
        /// Gets or sets Undo command.
        /// </summary>
        public static ICommand UndoCommand { get; set; }

        /// <summary>
        /// Gets or sets Redo command.
        /// </summary>
        public static ICommand RedoCommand { get; set; }

        /// <summary>
        /// Gets or sets CopyAsEmf command.
        /// </summary>
        public static ICommand CopyAsEmfCommand { get; set; }

        /// <summary>
        /// Gets or sets Cut command.
        /// </summary>
        public static ICommand CutCommand { get; set; }

        /// <summary>
        /// Gets or sets Copy command.
        /// </summary>
        public static ICommand CopyCommand { get; set; }

        /// <summary>
        /// Gets or sets Paste command.
        /// </summary>
        public static ICommand PasteCommand { get; set; }

        /// <summary>
        /// Gets or sets Delete command.
        /// </summary>
        public static ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Gets or sets SelectAll command.
        /// </summary>
        public static ICommand SelectAllCommand { get; set; }

        /// <summary>
        /// Gets or sets DeselectAll command.
        /// </summary>
        public static ICommand DeselectAllCommand { get; set; }

        /// <summary>
        /// Gets or sets ClearAll command.
        /// </summary>
        public static ICommand ClearAllCommand { get; set; }

        /// <summary>
        /// Gets or sets Cancel command.
        /// </summary>
        public static ICommand CancelCommand { get; set; }

        /// <summary>
        /// Gets or sets Group command.
        /// </summary>
        public static ICommand GroupCommand { get; set; }

        /// <summary>
        /// Gets or sets Ungroup command.
        /// </summary>
        public static ICommand UngroupCommand { get; set; }

        /// <summary>
        /// Gets or sets BringToFront command.
        /// </summary>
        public static ICommand BringToFrontCommand { get; set; }

        /// <summary>
        /// Gets or sets BringForward command.
        /// </summary>
        public static ICommand BringForwardCommand { get; set; }

        /// <summary>
        /// Gets or sets SendBackward command.
        /// </summary>
        public static ICommand SendBackwardCommand { get; set; }

        /// <summary>
        /// Gets or sets SendToBack command.
        /// </summary>
        public static ICommand SendToBackCommand { get; set; }

        /// <summary>
        /// Gets or sets MoveUp command.
        /// </summary>
        public static ICommand MoveUpCommand { get; set; }

        /// <summary>
        /// Gets or sets MoveDown command.
        /// </summary>
        public static ICommand MoveDownCommand { get; set; }

        /// <summary>
        /// Gets or sets MoveLeft command.
        /// </summary>
        public static ICommand MoveLeftCommand { get; set; }

        /// <summary>
        /// Gets or sets MoveRight command.
        /// </summary>
        public static ICommand MoveRightCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolNone command.
        /// </summary>
        public static ICommand ToolNoneCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolSelection command.
        /// </summary>
        public static ICommand ToolSelectionCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolPoint command.
        /// </summary>
        public static ICommand ToolPointCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolLine command.
        /// </summary>
        public static ICommand ToolLineCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolArc command.
        /// </summary>
        public static ICommand ToolArcCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolCubicBezier command.
        /// </summary>
        public static ICommand ToolCubicBezierCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolQuadraticBezier command.
        /// </summary>
        public static ICommand ToolQuadraticBezierCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolPath command.
        /// </summary>
        public static ICommand ToolPathCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolRectangle command.
        /// </summary>
        public static ICommand ToolRectangleCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolEllipse command.
        /// </summary>
        public static ICommand ToolEllipseCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolText command.
        /// </summary>
        public static ICommand ToolTextCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolImage command.
        /// </summary>
        public static ICommand ToolImageCommand { get; set; }

        /// <summary>
        /// Gets or sets ToolMove command.
        /// </summary>
        public static ICommand ToolMoveCommand { get; set; }

        /// <summary>
        /// Gets or sets ResetTool command.
        /// </summary>
        public static ICommand ResetToolCommand { get; set; }

        /// <summary>
        /// Gets or sets DefaultIsStroked command.
        /// </summary>
        public static ICommand DefaultIsStrokedCommand { get; set; }

        /// <summary>
        /// Gets or sets DefaultIsFilled command.
        /// </summary>
        public static ICommand DefaultIsFilledCommand { get; set; }

        /// <summary>
        /// Gets or sets DefaultIsClosed command.
        /// </summary>
        public static ICommand DefaultIsClosedCommand { get; set; }

        /// <summary>
        /// Gets or sets DefaultIsSmoothJoin command.
        /// </summary>
        public static ICommand DefaultIsSmoothJoinCommand { get; set; }

        /// <summary>
        /// Gets or sets SnapToGrid command.
        /// </summary>
        public static ICommand SnapToGridCommand { get; set; }

        /// <summary>
        /// Gets or sets TryToConnect command.
        /// </summary>
        public static ICommand TryToConnectCommand { get; set; }

        /// <summary>
        /// Gets or sets CloneStyle command.
        /// </summary>
        public static ICommand CloneStyleCommand { get; set; }

        /// <summary>
        /// Gets or sets AddDatabase command.
        /// </summary>
        public static ICommand AddDatabaseCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveDatabase command.
        /// </summary>
        public static ICommand RemoveDatabaseCommand { get; set; }

        /// <summary>
        /// Gets or sets AddColumn command.
        /// </summary>
        public static ICommand AddColumnCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveColumn command.
        /// </summary>
        public static ICommand RemoveColumnCommand { get; set; }

        /// <summary>
        /// Gets or sets AddRecord command.
        /// </summary>
        public static ICommand AddRecordCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveRecord command.
        /// </summary>
        public static ICommand RemoveRecordCommand { get; set; }

        /// <summary>
        /// Gets or sets ResetRecord command.
        /// </summary>
        public static ICommand ResetRecordCommand { get; set; }

        /// <summary>
        /// Gets or sets ApplyRecord command.
        /// </summary>
        public static ICommand ApplyRecordCommand { get; set; }

        /// <summary>
        /// Gets or sets AddShape command.
        /// </summary>
        public static ICommand AddShapeCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveShape command.
        /// </summary>
        public static ICommand RemoveShapeCommand { get; set; }

        /// <summary>
        /// Gets or sets AddProperty command.
        /// </summary>
        public static ICommand AddPropertyCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveProperty command.
        /// </summary>
        public static ICommand RemovePropertyCommand { get; set; }

        /// <summary>
        /// Gets or sets AddGroupLibrary command.
        /// </summary>
        public static ICommand AddGroupLibraryCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveGroupLibrary command.
        /// </summary>
        public static ICommand RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// Gets or sets AddGroup command.
        /// </summary>
        public static ICommand AddGroupCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveGroup command.
        /// </summary>
        public static ICommand RemoveGroupCommand { get; set; }

        /// <summary>
        /// Gets or sets InsertGroup command.
        /// </summary>
        public static ICommand InsertGroupCommand { get; set; }

        /// <summary>
        /// Gets or sets AddLayer command.
        /// </summary>
        public static ICommand AddLayerCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveLayer command.
        /// </summary>
        public static ICommand RemoveLayerCommand { get; set; }

        /// <summary>
        /// Gets or sets AddStyleLibrary command.
        /// </summary>
        public static ICommand AddStyleLibraryCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveStyleLibrary command.
        /// </summary>
        public static ICommand RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// Gets or sets AddStyle command.
        /// </summary>
        public static ICommand AddStyleCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveStyle command.
        /// </summary>
        public static ICommand RemoveStyleCommand { get; set; }

        /// <summary>
        /// Gets or sets ApplyStyle command.
        /// </summary>
        public static ICommand ApplyStyleCommand { get; set; }

        /// <summary>
        /// Gets or sets AddTemplate command.
        /// </summary>
        public static ICommand AddTemplateCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveTemplate command.
        /// </summary>
        public static ICommand RemoveTemplateCommand { get; set; }

        /// <summary>
        /// Gets or sets EditTemplate command.
        /// </summary>
        public static ICommand EditTemplateCommand { get; set; }

        /// <summary>
        /// Gets or sets ApplyTemplate command.
        /// </summary>
        public static ICommand ApplyTemplateCommand { get; set; }

        /// <summary>
        /// Gets or sets AddImageKey command.
        /// </summary>
        public static ICommand AddImageKeyCommand { get; set; }

        /// <summary>
        /// Gets or sets RemoveImageKey command.
        /// </summary>
        public static ICommand RemoveImageKeyCommand { get; set; }

        /// <summary>
        /// Gets or sets SelectedItemChanged command.
        /// </summary>
        public static ICommand SelectedItemChangedCommand { get; set; }

        /// <summary>
        /// Gets or sets AddPage command.
        /// </summary>
        public static ICommand AddPageCommand { get; set; }

        /// <summary>
        /// Gets or sets InsertPageBefore command.
        /// </summary>
        public static ICommand InsertPageBeforeCommand { get; set; }

        /// <summary>
        /// Gets or sets InsertPageAfter command.
        /// </summary>
        public static ICommand InsertPageAfterCommand { get; set; }

        /// <summary>
        /// Gets or sets AddDocument command.
        /// </summary>
        public static ICommand AddDocumentCommand { get; set; }

        /// <summary>
        /// Gets or sets InsertDocumentBefore command.
        /// </summary>
        public static ICommand InsertDocumentBeforeCommand { get; set; }

        /// <summary>
        /// Gets or sets InsertDocumentAfter command.
        /// </summary>
        public static ICommand InsertDocumentAfterCommand { get; set; }

        /// <summary>
        /// Gets or sets ZoomReset command.
        /// </summary>
        public static ICommand ZoomResetCommand { get; set; }

        /// <summary>
        /// Gets or sets ZoomAutoFit command.
        /// </summary>
        public static ICommand ZoomAutoFitCommand { get; set; }

        /// <summary>
        /// Gets or sets LoadWindowLayout command.
        /// </summary>
        public static ICommand LoadWindowLayoutCommand { get; set; }

        /// <summary>
        /// Gets or sets SaveWindowLayout command.
        /// </summary>
        public static ICommand SaveWindowLayoutCommand { get; set; }

        /// <summary>
        /// Gets or sets ResetWindowLayout command.
        /// </summary>
        public static ICommand ResetWindowLayoutCommand { get; set; }

        /// <summary>
        /// Gets or sets ObjectBrowser command.
        /// </summary>
        public static ICommand ObjectBrowserCommand { get; set; }

        /// <summary>
        /// Gets or sets DocumentViewer command.
        /// </summary>
        public static ICommand DocumentViewerCommand { get; set; }

        /// <summary>
        /// Gets or sets AboutDialog command.
        /// </summary>
        public static ICommand AboutDialogCommand { get; set; }

        /// <summary>
        /// Gets or sets ChangeCurrentView command.
        /// </summary>
        public static ICommand ChangeCurrentViewCommand { get; set; }

        /// <summary>
        /// Initialize commands.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            NewCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnNew(p));
            OpenCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnOpen(p as string));
            CloseCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnCloseProject());
            SaveCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnSave());
            SaveAsCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnSaveAs());
            ExecuteScriptCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnExecuteScript(p as string));
            ImportObjectCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnImportObject(p as string));
            ExportObjectCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnExportObject(p));
            ImportXamlCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnImportXaml(p as string));
            ExportXamlCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnExportXaml(p));
            ImportJsonCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnImportJson(p as string));
            ExportJsonCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnImportJson(p as string));
            ExportCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnExport(p));
            ExitCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnExit());
            ImportDataCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnImportData(p as IProjectContainer));
            ExportDataCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnExportData(p as IDatabase));
            UpdateDataCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnUpdateData(p as IDatabase));

            UndoCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnUndo());
            RedoCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRedo());
            CopyAsEmfCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnCopyAsEmf(p));
            CutCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnCut(p));
            CopyCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnCopy(p));
            PasteCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnPaste(p));
            DeleteCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnDelete(p));
            SelectAllCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnSelectAll());
            DeselectAllCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnDeselectAll());
            ClearAllCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnClearAll());
            CancelCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnCancel());
            GroupCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnGroupSelected());
            UngroupCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnUngroupSelected());
            BringToFrontCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnBringToFrontSelected());
            BringForwardCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnBringForwardSelected());
            SendBackwardCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnSendBackwardSelected());
            SendToBackCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnSendToBackSelected());
            MoveUpCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnMoveUpSelected());
            MoveDownCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnMoveDownSelected());
            MoveLeftCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnMoveLeftSelected());
            MoveRightCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnMoveRightSelected());

            ToolNoneCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolNone());
            ToolSelectionCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolSelection());
            ToolPointCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolPoint());
            ToolLineCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolLine());
            ToolArcCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolArc());
            ToolCubicBezierCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolCubicBezier());
            ToolQuadraticBezierCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolQuadraticBezier());
            ToolPathCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolPath());
            ToolRectangleCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolRectangle());
            ToolEllipseCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolEllipse());
            ToolTextCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolText());
            ToolImageCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolImage());
            ToolMoveCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToolMove());
            ResetToolCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnResetTool());

            DefaultIsStrokedCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleDefaultIsStroked());
            DefaultIsFilledCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleDefaultIsFilled());
            DefaultIsClosedCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleDefaultIsClosed());
            DefaultIsSmoothJoinCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleDefaultIsSmoothJoin());
            SnapToGridCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleSnapToGrid());
            TryToConnectCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleTryToConnect());
            CloneStyleCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnToggleCloneStyle());

            AddDatabaseCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddDatabase());
            RemoveDatabaseCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveDatabase(p as IDatabase));
            AddColumnCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddColumn(p as IDatabase));
            RemoveColumnCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveColumn(p as IColumn));
            AddRecordCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddRecord(p as IDatabase));
            RemoveRecordCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveRecord(p as IRecord));
            ResetRecordCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnResetRecord(p as IContext));
            ApplyRecordCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnApplyRecord(p as IRecord));
            AddShapeCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddShape(p as IBaseShape));
            RemoveShapeCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveShape(p as IBaseShape));
            AddPropertyCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddProperty(p as IContext));
            RemovePropertyCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveProperty(p as IProperty));
            AddGroupLibraryCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddGroupLibrary());
            RemoveGroupLibraryCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveGroupLibrary(p as ILibrary<IGroupShape>));
            AddGroupCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddGroup(p as ILibrary<IGroupShape>));
            RemoveGroupCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveGroup(p as IGroupShape));
            InsertGroupCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnInsertGroup(p as IGroupShape));
            AddLayerCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddLayer(p as IPageContainer));
            RemoveLayerCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveLayer(p as ILayerContainer));
            AddStyleLibraryCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddStyleLibrary());
            RemoveStyleLibraryCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveStyleLibrary(p as ILibrary<IShapeStyle>));
            AddStyleCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddStyle(p as ILibrary<IShapeStyle>));
            RemoveStyleCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveStyle(p as IShapeStyle));
            ApplyStyleCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnApplyStyle(p as IShapeStyle));
            AddTemplateCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddTemplate());
            RemoveTemplateCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveTemplate(p as IPageContainer));
            EditTemplateCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnEditTemplate(p as IPageContainer));
            ApplyTemplateCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnApplyTemplate(p as IPageContainer));
            AddImageKeyCommand = new Command((p) => true, async (p) => await serviceProvider.GetService<IProjectEditor>().OnAddImageKey(p as string));
            RemoveImageKeyCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnRemoveImageKey(p as string));
            SelectedItemChangedCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnSelectedItemChanged(p as IObservableObject));
            AddPageCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddPage(p));
            InsertPageBeforeCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnInsertPageBefore(p));
            InsertPageAfterCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnInsertPageAfter(p));
            AddDocumentCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnAddDocument(p));
            InsertDocumentBeforeCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnInsertDocumentBefore(p));
            InsertDocumentAfterCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().OnInsertDocumentAfter(p));

            ZoomResetCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnZoomReset());
            ZoomAutoFitCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnZoomAutoFit());

            LoadWindowLayoutCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnLoadLayout());
            SaveWindowLayoutCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnSaveLayout());
            ResetWindowLayoutCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnResetLayout());

            ObjectBrowserCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnObjectBrowser());
            DocumentViewerCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnDocumentViewer());
            AboutDialogCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Platform.OnAboutDialog());

            ChangeCurrentViewCommand = new Command((p) => true, (p) => serviceProvider.GetService<IProjectEditor>().Layout?.Navigate(p));
        }
    }
}
