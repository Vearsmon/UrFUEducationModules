// Клик на затемнитель убирает окно редактирования и создания ОП
const shadow = document.getElementsByClassName("details-window-background-shadow");
if (shadow.length !== 0) {
    shadow[0].addEventListener('click', () => {
        document.getElementsByClassName("details-window")[0].hidden = true;
        document.getElementsByClassName("field_multiselect")[0].innerHTML = "";
        // При закрытии убираем кнопку, которая может быть не нужна при создании ОП
        const deleteBtn = document.getElementsByClassName("delete-button");
        if (deleteBtn.length !== 0) {
            deleteBtn[0].remove();
        }
    });
}

// "Оживляем" кнопку добавления ОП
let addProgramButton = document.getElementById("add-program")
if (addProgramButton !== null) {
    addProgramButton.addEventListener('click', async () => {
        await openAddProgramWindow()
    })
}

// Делаем окно редактирования ОП видимым
async function openDetailsWindow(programId) {
    // Неавторизованным пользователям не разрешается редактировать
    let user = "";
    await fetch('api/isAuthenticated', {method: 'get'})
        .then((response) => response.text())
        .then((userId) => {
            if (userId !== "false") {
                user = userId
            }
        });

    // Строим форму по полученным из API данным
    const getProgramData = new URL('http://localhost:5039/api/get/program');
    getProgramData.search = new URLSearchParams(programId).toString();
    await fetch(getProgramData, { method: 'get' })
        .then((response) => response.json())
        .then(async (infoParams) => {
            if (user !== "") {
                document.getElementById("uuid").value = infoParams["uuid"];
                await loadMainInfo();

                document.getElementById("title-input").value = infoParams["title"];
                document.getElementById("status-input").value = infoParams["status"];
                document.getElementById("cypher-input").value = infoParams["cypher"];
                document.getElementById("level-input").value = infoParams["level"];
                document.getElementById("standard-input").value = infoParams["standard"];
                document.getElementById("institute-input").value = infoParams["institute"];
                document.getElementById("head-input").value = infoParams["head"];
                document.getElementById("accreditationTime-input").value = infoParams["accreditationTime"].slice(0,10)

                // Добавляем опции выбора модулей для соответствующей ОП
                let modulesBlock = document.getElementById("select-1");
                modulesBlock.innerText = "";
                modulesBlock.innerHTML = "";
                let modules = await fetchTitles("api/get/modules");
                for (let module of modules) {
                    const option = document.createElement("option");
                    option.setAttribute("id", module);
                    option.setAttribute("value", module);
                    option.innerText = module;
                    modulesBlock.appendChild(option);
                }

                // Помечаем какие опции выбора модуля являются уже привязанными к данной ОП
                const linkedModules = await getLinkedModules(programId);
                if (linkedModules.length !== 0) {
                    document.getElementsByClassName("field_multiselect")[0].innerText = "";
                }

                await addDropDownOptions();
                
                for (let module of linkedModules) {
                    const label = document.getElementsByClassName("field_multiselect");
                    const button = document.createElement("button");
                    document.getElementById(module).selected = true;
                    button.setAttribute("type", "button");
                    button.setAttribute("class", "btn_multiselect")
                    button.setAttribute("id", `button-${module}`)
                    button.innerText = module;
                    button.addEventListener('click', () => {
                        document.getElementById(module).selected = false;
                        document.getElementById(`button-${module}`).remove();
                    });

                    document.getElementsByClassName("field_multiselect")[0].appendChild(button);
                }

                // Настраиваем кнопку удаления здесь, потому что она нужна только в одной форме из двух
                const deleteButton = document.createElement("button");
                deleteButton.setAttribute("class", "delete-button button-9");
                deleteButton.setAttribute("role", "button");
                deleteButton.innerText = "Удалить программу";
                document.getElementsByClassName("details-window-form")[0].appendChild(deleteButton);
                deleteButton.addEventListener("click", () => {
                    const getModulesData = new URL('http://localhost:5039/api/delete/program');
                    getModulesData.search = new URLSearchParams(programId).toString();
                    fetch(getModulesData, { method: 'post', body: programId})
                        .then(() => {
                            window.location.reload();
                        });
                });

                // Делаем окно видимым
                document.getElementsByClassName("details-window")[0].hidden = false;
            }
        });
}

// Конфигурируем поля ввода для формы редактирования/создания ОП
async function loadMainInfo() {
    document.getElementById("title").innerHTML = `<input type="text" id="title-input" class="window-input">`;
    document.getElementById("status").innerHTML = `<input type="text" id="status-input" class="window-input">`;
    document.getElementById("cypher").innerHTML = `<input type="text" id="cypher-input" class="window-input">`;
    await insertTitles(await fetchTitles("api/get/levels"), "level-input", "level")
    await insertTitles(await fetchTitles("api/get/standards"), "standard-input", "standard")
    await insertTitles(await fetchTitles("api/get/institutes"), "institute-input", "institute")
    await insertTitles(await fetchTitles("api/get/heads"), "head-input", "head")
    document.getElementById("accreditationTime").innerHTML = `<input type="date" id="accreditationTime-input" class="window-input">`;
}

// Получаем по запросу к API список интересующих названий какой-либо
// категории (названия институтов, ответственных лиц, стандартов обучения и т.д.)
async function fetchTitles(url) {
    return await fetch(url, {method: 'get'})
        .then((response) => response.json())
        .then((titles) => {
            return titles
        });
}

// Полученные данные вставляем в выпадающие меню
async function insertTitles(titles, selectorId, selectorName) {
    const select = document.createElement("select");
    select.setAttribute("id", `${selectorId}`)
    document.getElementById(selectorName).innerHTML = ""
    document.getElementById(selectorName).appendChild(select)

    const option = document.createElement("option");
    option.disabled = true;
    option.selected = true;
    option.innerText = "-";
    document.getElementById(selectorId).appendChild(option)

    for (let title of titles) {
        title = title.toString('utf8');
        let alreadyExists = false;
        let select = document.getElementById(selectorId);
        for (let option of select.options) {
            if (option.innerText === title) {
                alreadyExists = true;
                break;
            }
        }
        if (!alreadyExists) {
            const option = document.createElement("option");
            option.setAttribute("value", title)
            option.innerText = title;
            document.getElementById(selectorId).appendChild(option)
        }
    }
}

// Получаем все модули, связанные с конкретной ОП
async function getLinkedModules(programId) {
    const getModulesData = new URL('http://localhost:5039/api/get/linkedModules');
    getModulesData.search = new URLSearchParams(programId).toString();
    return await fetch(getModulesData, { method: 'get' })
        .then((response) => response.json())
        .then(async (modules) => {
            return modules
        });
}

// Делаем очень запаристое выпадающее меню, которое удобно показывает уже прикрепленные к ОП модули
// или модули, которые хочется добавить
async function addDropDownOptions() {
    let multiselect_block = document.getElementsByClassName("multiselect_block");
    for (let parent of multiselect_block) {
        let label = parent.getElementsByClassName("field_multiselect")[0];
        let select = parent.getElementsByClassName("field_select")[0];
        let text = label.innerHTML;
        select.addEventListener("change", function(element) {
            let selectedOptions = this.selectedOptions;
            label.innerHTML = "";
            for (let option of selectedOptions) {
                let button = document.createElement("button");
                button.type = "button";
                button.className = "btn_multiselect";
                button.textContent = option.value;
                button.onclick = _ => {
                    option.selected = false;
                    button.remove();
                    if (!select.selectedOptions.length) label.innerHTML = text
                };
                label.append(button);
            }
        })
    }
}

// Почти такое же окно, как и для редактирования, только с пустой формой
async function openAddProgramWindow() {
    
    await loadMainInfo();
    let modulesBlock = document.getElementById("select-1");
    modulesBlock.innerText = "";
    modulesBlock.innerHTML = "";
    let modules = await fetchTitles("api/get/modules");
    for (let module of modules) {
        const option = document.createElement("option");
        option.setAttribute("value", module);
        option.innerText = module;
        modulesBlock.appendChild(option);
    }

    document.getElementsByClassName("details-window")[0].hidden = false;
}

// Собираем со всех блоков данные в одну удобную формочку для API
function buildProgramForm() {
    if (isAnyFieldEmpty()) {
        alert("Вы заполнили не все поля")
        return null;
    }
    let formData = new FormData();
    formData.append('uuid', document.getElementById("uuid").value);
    formData.append('title', document.getElementById("title-input").value);
    formData.append('status', document.getElementById("status-input").value);
    formData.append('cypher', document.getElementById("cypher-input").value);
    formData.append('level', document.getElementById("level-input").value);
    formData.append('standard', document.getElementById("standard-input").value);
    formData.append('institute', document.getElementById("institute-input").value);
    formData.append('head', document.getElementById("head-input").value);
    formData.append('accreditationTime', document.getElementById("accreditationTime-input").value);
    let modulesTitles = "";
    const selectedModules = document.getElementsByClassName("btn_multiselect");
    for (let module of selectedModules) {
        modulesTitles += module.innerText + "@"
    }
    formData.append('modules', modulesTitles.slice(0, modulesTitles.length))
    return formData
}

// В условии сказано, что все поля должны быть обязательными, поэтому ограничиваем пользователя, если какое-то поле
// оказалось незаполненным. Этот метод проверяет это.
function isAnyFieldEmpty() {
    const elements = [document.getElementById("title-input").value,
        document.getElementById("status-input").value,
        document.getElementById("cypher-input").value,
        document.getElementById("level-input").value,
        document.getElementById("standard-input").value,
        document.getElementById("institute-input").value,
        document.getElementById("head-input").value,
        document.getElementById("accreditationTime-input").value]

    return elements.some((el) => el === "");
}