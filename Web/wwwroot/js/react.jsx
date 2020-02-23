class Case extends React.Component {
    handleFormToggle = e => {
        this.props.toggleCaseForm("edit", this.props.caseId);
    }
    handleDeleteConfirm = e => {
        this.props.showConfirmModal(this.props.caseId);
    }
    render() {
        return (
            <tr className="case">
                <td>
                    {this.props.description}
                </td>
                <td>
                    {this.props.caseNumber}
                </td>
                <td>
                    {this.props.kindType}
                </td>
                <td>
                    {this.props.status}
                </td>
                <td>
                    {this.props.title}
                </td>
                <td>
                    <button type="button" className="btn btn-info" data-target="#caseModal"
                        onClick={this.handleFormToggle}>Edit</button>
                    <button type="button" className="btn btn-danger"
                        onClick={this.handleDeleteConfirm}>Delete</button>
                </td>
            </tr>
        );
    }
}

class CaseList extends React.Component {
    render() {
        const caseNodes = this.props.data && this.props.data.map(c => (
            <Case description={c.description} key={c.id} caseId={c.id} caseNumber={c.caseNumber}
                kindType={c.kind} status={c.status} title={c.title} toggleCaseForm={this.props.toggleCaseForm}
                showConfirmModal={this.props.showConfirmModal}>
            </Case>
        ));
        return (
            <tbody className="caseList">{caseNodes}</tbody>
        );
    }
}

class CaseInput extends React.Component {
    handleChange = e => {
        this.props.onChange(this.props.id, e.target.value);
    }
    render() {
        return (
            <div className="form-group">
                <label className="control-label">{this.props.label}</label>
                <input className="form-control" value={this.props.value} onChange={this.handleChange} />
            </div>
        )
    }
}

class DeleteConfirmModal extends React.Component {
    handleDeleteConfirm = e => {
        this.props.handleDelete();
    }

    handleModalToggle = e => {
        this.props.toggleConfirmModal();
    }

    render() {
        return (
            <div className="modal fade" id="confirmModal" role="dialog" data-backdrop="static" data-keyboard="false">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" onClick={this.handleModalToggle}>&times;</button>
                            <h4 className="modal-title">Delete case?</h4>
                        </div>
                        <form onSubmit={this.handleDeleteConfirm}>
                            <div className="modal-footer">
                                <input type="submit" className="btn btn-default" value="Delete"/>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        )
    }
}

class CaseForm extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            caseData: this.props._case
        }

        this.handleFieldChange = this.handleFieldChange.bind(this);
    }

    handleFormToggle = e => {
        this.props.toggleCaseForm("none", this.props.caseId);
    }

    handleCaseEdit = e => {
        this.props.onCaseEdit(this.state.caseData);
    }

    handleCaseCreate = e => {
        this.props.onCaseCreate(this.state.caseData);
    }

    handleFieldChange(fieldId, value) {
        this.setState(state => ({
            caseData: {
                ...state.caseData,
                [fieldId]: value
            }
        }))
    }

    render() {
        const caseInputs = Object.entries(this.state.caseData).map((key,i) => (
            <CaseInput
                key={key[0]}
                id={key[0]}
                onChange={this.handleFieldChange}
                value={key[1]}
                label={this.props.inputLabels[i]}>
            </CaseInput>
        ));
        
        return (
            <div className="modal fade" id="caseModal" role="dialog" data-backdrop="static" data-keyboard="false">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" onClick={this.handleFormToggle}>&times;</button>
                            <h4 className="modal-title">{this.props.modalTitle}</h4>
                        </div>
                        <form onSubmit={this.props.formMode == "create" ? this.handleCaseCreate : this.handleCaseEdit}>
                            <div className="modal-body">
                                {caseInputs}
                            </div>

                            <div className="modal-footer">
                                <input type="submit" className="btn btn-default" value="Submit" />
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        );
    }
}

class CaseBox extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.props.initialData,
            formMode: "none",
            showDeleteConfirm: false
        };
        this.editCase = this.editCase.bind(this);
        this.createCase = this.createCase.bind(this);
        this.deleteCase = this.deleteCase.bind(this);
        this.toggleCaseForm = this.toggleCaseForm.bind(this);
        this.handleFormToggle = this.handleFormToggle.bind(this);
        this.toggleConfirmModal = this.toggleConfirmModal.bind(this);
        
    }
    loadCasesFromServer() {
        const xhr = new XMLHttpRequest();
        xhr.open('get', this.props.getUrl, true);
        xhr.onload = function () {
            const data = JSON.parse(xhr.responseText);
            this.setState({ data: data });
        }.bind(this);
        xhr.send();
    }

    editCase(_case) {
        $.ajax({
            url: this.props.editUrl,
            data: _case,
            type: "post",
            success: function(data) {
                console.log(data)
            },
            error: function (xhr, ajaxOptions, thrownError){
                console.log(xhr.responseText);
            }    
        })
    }

    createCase(_case) {
        $.ajax({
            url: this.props.createUrl,
            data: _case,
            type: "post",
            success: function(data) {
                console.log(data)
            },
            error: function (xhr, ajaxOptions, thrownError){
                console.log(xhr.responseText);
            }    
        })
    }

    deleteCase() {
        $.ajax({
            url: this.props.deleteUrl,
            type: "post",
            contentType: "application/json",
            data: JSON.stringify(this.state.deletingCaseId),
            success: function(data) {
                console.log(data)
            },
            error: function (xhr, ajaxOptions, thrownError){
                console.log(xhr.responseText);
            }    
        })
    }

    componentDidMount() {
        this.loadCasesFromServer();
    }

    componentDidUpdate() {
        if (this.state.formMode == "edit" || this.state.formMode == "create") {
            $('#caseModal').modal('show')
        }
        if (this.state.showDeleteConfirm) {
            $('#confirmModal').modal('show')
        }
        else {
            $('#confirmModal').modal('hide')
        }
    }

    toggleCaseForm(caller, id) {
        if (caller == "edit") {
            //filter returns an array so we take the first element
            // TODO: set up a proper check if somehow the filter fails
            const c = (this.state.data.filter(function (c) {
                return c.id == id
            })[0]);
            this.setState({
                formMode: "edit",
                modalTitle: "Edit case",
                _case: c
            })
        }
        else if (caller == "create") {
            const c = {...this.state.data[0]};
            Object.keys(c).forEach(function(key) {
                 c[key] = ""
            })
            this.setState({
                formMode: "create",
                modalTitle: "Create case",
                _case: c
            })
        }
        else {
            this.setState({
                formMode: "none"
            })
        }
    }

    toggleConfirmModal(d) {
        if (this.state.showDeleteConfirm) {
            this.setState({
                showDeleteConfirm: false,
                deletingCaseId: -1
            })
        }
        else {
            this.setState({
                showDeleteConfirm: true,
                deletingCaseId: d
            })
        }
    }

    handleFormToggle() {
        this.toggleCaseForm("create");
    }

    render() {
        return (
            <div className="caseBox">
                <h3>Cases</h3>
                <button type="button" className="btn btn-info" data-target="#caseModal" onClick={this.handleFormToggle}>Create new case</button>
                <table className="table">
                    <thead>
                        <tr style={{ fontWeight: "bold" }}>
                            {/* we skip the id label */}
                            {this.props.labels.slice(1).map(l => {
                                return <td>{l}</td>
                            })}
                        </tr>
                    </thead>
                    <CaseList data={this.state.data} toggleCaseForm={this.toggleCaseForm} showConfirmModal={this.toggleConfirmModal}/>
                </table>
                {
                    (this.state.formMode != "none") &&
                    <CaseForm onCaseEdit={this.editCase} onCaseCreate={this.createCase} modalTitle={this.state.modalTitle} _case={this.state._case}
                        formMode={this.state.formMode} toggleCaseForm={this.toggleCaseForm} inputLabels={this.props.labels}/>
                }
                {
                    (this.state.showDeleteConfirm) && 
                    <DeleteConfirmModal handleDelete={this.deleteCase} toggleConfirmModal={this.toggleConfirmModal}></DeleteConfirmModal>
                }
            </div>
        );
    }
}
