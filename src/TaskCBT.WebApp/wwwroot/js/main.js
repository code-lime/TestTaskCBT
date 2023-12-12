const claims = {
    role: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
}

/**
 * @param {string} api
*/
async function preloadUser(api) {
    initFetchJwt(`${api}/auth/refresh`, () => window.location = 'auth');
    let jwt = getParsedCurrentJwt();
    if (jwt === undefined) {
        window.location = 'auth';
        return undefined;
    }
    if (claims.role in jwt) {
        let role = jwt[claims.role];
        if (role === 'create') {
            if (window.location.href !== 'create') {
                window.location = 'create';
            }
            return undefined;
        }
    }
    let response = await fetch(`${api}/users/0`, { jwt: 'local' });
    if (response.status !== 200) {
        window.location = 'auth';
        return undefined;
    }
    let json = await response.json();
    if (json.status !== 'success' || !('response' in json)) {
        window.location = 'auth';
        return undefined;
    }
    return json.response;
}
/**
 * @param {string} api
*/
function preloadCreate(api) {
    initFetchJwt(`${api}/auth/refresh`, () => window.location = 'auth');
    let jwt = getParsedCurrentJwt();
    if (jwt === undefined) {
        window.location = 'auth';
        return false;
    }
    if (claims.role in jwt && jwt[claims.role] === 'create')
        return true;
    window.location = '/';
    return false;
}
/**
 * @param {{tag:string, options?:ElementCreationOptions, child?:tree, childs?:Array<tree>, setup?:function(HTMLElement)}} tree
*/
function createTree(tree) {
    let current = document.createElement(tree.tag, tree.options);
    if ('child' in tree) {
        current.appendChild(createTree(tree.child));
    }
    if ('childs' in tree) {
        for (let child of tree.childs)
            current.appendChild(createTree(child));
    }
    if ('setup' in tree) {
        tree.setup(current);
    }
    return current;
}

function deepClone(obj) {
    return JSON.parse(JSON.stringify(obj));
}

function isObject(obj) {
    return typeof obj === 'object' && obj !== null;
}

/**
 * @param {HTMLElement} table
 * @param {{key: string}} params
 * @param {false|function({key: (string|{type?: string, value: string})}):void} changeCallback
 * @param {({buttonName:string}|null)} singleCallback
*/
function createParamsTable(table, params, changeCallback, singleCallback = null) {
    params = deepClone(params);
    /**@type {Array<string>}*/
    let paramsEntries = Object.entries(params);

    table.innerHTML = '';

    let paramInputs = {};

    for (let [paramName, paramValue] of paramsEntries) {
        let optional = paramName.startsWith('?');
        if (optional) paramName = paramName.substring(1);
        /**@type {HTMLElement}*/
        let valueInput;
        table.appendChild(createTree({
            tag: 'tr',
            childs: [
                {
                    tag: 'td',
                    setup: function (element) {
                        element.textContent = paramName + ((optional || changeCallback === undefined) ? '' : '*');
                    }
                },
                {
                    tag: 'td',
                    childs: changeCallback === false ? [] : [{
                        tag: 'input',
                        setup: function (element) {
                            valueInput = element;
                            if (isObject(paramValue)) {
                                element.setAttribute('type', 'type' in paramValue ? paramValue.type : 'text');
                                element.setAttribute('value', paramValue.value);
                            } else {
                                element.setAttribute('type', 'text');
                                element.setAttribute('value', paramValue);
                            }
                            element.style.height = '100%';
                            element.style.width = '100%';
                        }
                    }],
                    setup: function (element) {
                        if (changeCallback === false) {
                            if (isObject(paramValue)) {
                                element.textContent = paramValue.value;
                            } else {
                                element.textContent = paramValue;
                            }
                        }
                    }
                },
                ...(singleCallback !== null || changeCallback === false ? [] : [{
                    tag: 'td',
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            element.setAttribute('type', 'button');
                            element.setAttribute('value', '$');
                            element.style.backgroundColor = '#00ff00';
                            element.style.height = '30px';
                            element.style.width = '30px';

                            element.onclick = function () {
                                /**@type {string}*/
                                let valueText = valueInput.value;
                                valueText = valueText.trim();
                                if (!optional && valueText.length === 0) {
                                    alert('Ошибка сохранения! Значение поля обязательно');
                                    return;
                                }
                                params[(optional ? '?' : '') + paramName] = valueText.length == 0 ? null : valueText;
                                changeCallback(params);
                            }
                        }
                    }
                }])
            ]
        }));
        paramInputs[paramName] = valueInput;
    }
    if (singleCallback !== null && changeCallback !== false) {
        let paramsKeys = Object.keys(params);
        table.appendChild(createTree({
            tag: 'tr',
            childs: [
                {
                    tag: 'td',
                    setup: function (element) {
                        element.setAttribute('colspan', '2');
                    },
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            element.setAttribute('type', 'button');
                            element.setAttribute('value', singleCallback.buttonName);
                            element.style.backgroundColor = '#00ff00';
                            element.style.height = '30px';

                            element.onclick = function () {
                                for (let paramName of paramsKeys) {
                                    let optional = paramName.startsWith('?');
                                    if (optional) paramName = paramName.substring(1);
                                    /**@type {HTMLElement}*/
                                    let paramInput = paramInputs[paramName];
                                    /**@type {string}*/
                                    let paramText = paramInput.value;
                                    paramText = paramText.trim();
                                    if (!optional && paramText.length === 0) {
                                        alert('Ошибка сохранения! Значение поля "' + paramName +'" обязательно');
                                        return;
                                    }
                                    params[(optional ? '?' : '') + paramName] = paramText.length === 0 ? null : paramText;
                                }
                                changeCallback(params);
                            }
                        }
                    }
                }
            ]
        }));
    }
}
/**
 * @param {HTMLElement} table
 * @param {{key: string}} fields
 * @param {false|function({key: string}):void} changeCallback
*/
function createFieldsTable(table, fields, changeCallback = false) {
    params = deepClone(params);
    /**@type {Array<string>}*/
    let fieldEntries = Object.entries(fields);

    table.innerHTML = '';

    for (let [fieldName, fieldValue] of fieldEntries) {
        table.appendChild(createTree({
            tag: 'tr',
            childs: [
                {
                    tag: 'td',
                    setup: function (element) {
                        element.textContent = fieldName;
                    }
                },
                {
                    tag: 'td',
                    setup: function (element) {
                        element.textContent = fieldValue;
                    }
                },
                ...(changeCallback === false ? [] : [{
                    tag: 'td',
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            element.setAttribute('type', 'button');
                            element.setAttribute('value', '-');
                            element.style.backgroundColor = '#ff0000';
                            element.style.height = '30px';
                            element.style.width = '30px';
                            element.onclick = function () {
                                delete fields[fieldName];
                                changeCallback(fields);
                            }
                        }
                    }
                }])
            ]
        }));
    }

    if (changeCallback === false) return;

    /**@type {HTMLElement}*/
    var keyInput;

    /**@type {HTMLElement}*/
    var valueInput;

    table.appendChild(createTree({
        tag: 'tr',
        childs: [
            {
                tag: 'td',
                child: {
                    tag: 'input',
                    setup: function (element) {
                        keyInput = element;
                        element.setAttribute('type', 'text');
                        element.setAttribute('value', '');
                        element.style.height = '100%';
                        element.style.width = '100%';
                    }
                }
            },
            {
                tag: 'td',
                child: {
                    tag: 'input',
                    setup: function (element) {
                        valueInput = element;
                        element.setAttribute('type', 'text');
                        element.setAttribute('value', '');
                        element.style.height = '100%';
                        element.style.width = '100%';
                    }
                }
            },
            {
                tag: 'td',
                child: {
                    tag: 'input',
                    setup: function (element) {
                        element.setAttribute('type', 'button');
                        element.setAttribute('value', '+');
                        element.style.backgroundColor = '#00ff00';
                        element.style.height = '30px';
                        element.style.width = '30px';
                        element.onclick = function () {
                            /**@type {string}*/
                            let keyText = keyInput.value;
                            keyText = keyText.trim();
                            /**@type {string}*/
                            let valueText = valueInput.value;
                            valueText = valueText.trim();
                            if (keyText.length === 0 || valueText.length === 0) {
                                alert('Ошибка сохранения! Название или значение поля пустое');
                                return;
                            }
                            if (keyText in fields) {
                                alert('Ошибка сохранения! Название поля совпадает с существующим полем');
                                return;
                            }
                            fields[keyText] = valueText.length === null ? null : valueText;
                            changeCallback(fields);
                        }
                    }
                }
            }
        ]
    }));
}
/**
 * @param {HTMLElement} table
 * @param {Array<{id:string, show:Array<string>}>} hrefs
 * @param {false|function(string):void} hrefCallback
*/
function createHrefTable(table, hrefs, hrefCallback = false) {
    params = deepClone(params);
    /**@type {Array<string>}*/

    table.innerHTML = '';

    for (let href of hrefs) {
        let showArray = [];
        let id = href.id;
        for (let showItem of href.show) {
            showArray.push({
                tag: 'td',
                setup: function (element) {
                    element.textContent = showItem;
                }
            });
        }
        table.appendChild(createTree({
            tag: 'tr',
            childs: [
                ...showArray,
                ...(hrefCallback === false ? [] : [{
                    tag: 'td',
                    child: {
                        tag: 'input',
                        setup: function (element) {
                            element.setAttribute('type', 'button');
                            element.setAttribute('value', 'Перейти');
                            element.onclick = function () {
                                hrefCallback(id);
                            }
                        }
                    }
                }])
            ]
        }));
    }
}