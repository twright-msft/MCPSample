// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// MCP Tools Testing JavaScript
document.addEventListener('DOMContentLoaded', function() {
    const echoInput = document.getElementById('echoInput');
    const testEchoBtn = document.getElementById('testEchoBtn');
    const echoResult = document.getElementById('echoResult');
    const listToolsBtn = document.getElementById('listToolsBtn');
    const toolsList = document.getElementById('toolsList');
    const calcA = document.getElementById('calcA');
    const calcOperation = document.getElementById('calcOperation');
    const calcB = document.getElementById('calcB');
    const testCalcBtn = document.getElementById('testCalcBtn');
    const calcResult = document.getElementById('calcResult');
    const wordCountText = document.getElementById('wordCountText');
    const includeSpaces = document.getElementById('includeSpaces');
    const testWordCountBtn = document.getElementById('testWordCountBtn');
    const wordCountResult = document.getElementById('wordCountResult');

    // Return early if echo elements don't exist (not on the right page)
    if (!echoInput || !testEchoBtn || !echoResult) {
        return;
    }

    // Echo tool functionality
    testEchoBtn.addEventListener('click', async function() {
        const inputText = echoInput.value.trim();
        
        if (!inputText) {
            showResult('Please enter some text to echo.', 'warning');
            return;
        }

        // Disable button and show loading state
        testEchoBtn.disabled = true;
        testEchoBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Testing...';
        
        try {
            const response = await fetch('/api/mcp/tools/call', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    method: 'tools/call',
                    name: 'echo',
                    arguments: {
                        text: inputText
                    }
                })
            });

            const data = await response.json();
            
            if (response.ok && data.result) {
                const content = data.result.content;
                if (content && content.length > 0) {
                    showResult(`✅ ${content[0].text}`, 'success');
                } else {
                    showResult('Echo tool returned empty result.', 'warning');
                }
            } else if (data.error) {
                showResult(`❌ Error: ${data.error.message}`, 'danger');
            } else {
                showResult('❌ Unexpected response format.', 'danger');
            }
        } catch (error) {
            showResult(`❌ Network error: ${error.message}`, 'danger');
        } finally {
            // Re-enable button
            testEchoBtn.disabled = false;
            testEchoBtn.innerHTML = 'Test Echo Tool';
        }
    });

    // Allow Enter key to trigger the test
    echoInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            testEchoBtn.click();
        }
    });

    // Tools list functionality
    if (listToolsBtn && toolsList) {
        listToolsBtn.addEventListener('click', async function() {
            // Disable button and show loading state
            listToolsBtn.disabled = true;
            listToolsBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Loading Tools...';
            
            try {
                const response = await fetch('/api/mcp/tools/list', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        method: 'tools/list'
                    })
                });

                const data = await response.json();
                
                if (response.ok && data.result && data.result.tools) {
                    const tools = data.result.tools;
                    if (tools.length > 0) {
                        let toolsHtml = '<h6>Available Tools:</h6><ul class="mb-0">';
                        tools.forEach(tool => {
                            toolsHtml += `<li><strong>${tool.name}</strong>`;
                            if (tool.description) {
                                toolsHtml += `: ${tool.description}`;
                            }
                            toolsHtml += '</li>';
                        });
                        toolsHtml += '</ul>';
                        showToolsResult(toolsHtml, 'success');
                    } else {
                        showToolsResult('No tools available.', 'warning');
                    }
                } else if (data.error) {
                    showToolsResult(`❌ Error: ${data.error.message}`, 'danger');
                } else {
                    showToolsResult('❌ Unexpected response format.', 'danger');
                }
            } catch (error) {
                showToolsResult(`❌ Network error: ${error.message}`, 'danger');
            } finally {
                // Re-enable button
                listToolsBtn.disabled = false;
                listToolsBtn.innerHTML = 'List Available Tools';
            }
        });
    }

    // Calculator functionality
    if (testCalcBtn && calcResult) {
        testCalcBtn.addEventListener('click', async function() {
            // Get fresh references to ensure elements exist
            const calcAElement = document.getElementById('calcA');
            const calcBElement = document.getElementById('calcB');
            const calcOpElement = document.getElementById('calcOperation');
            
            if (!calcAElement || !calcBElement || !calcOpElement) {
                showCalcResult('Calculator form elements not found. Please refresh the page.', 'danger');
                return;
            }
            
            const aValue = calcAElement.value;
            const bValue = calcBElement.value;
            const operation = calcOpElement.value;
            
            // Debug logging
            console.log('Calculator inputs:', { aValue, bValue, operation });
            
            // Check if values exist (including checking for default values)
            if (aValue === '' || bValue === '') {
                showCalcResult(`Please enter numbers for both operands. A: "${aValue}", B: "${bValue}"`, 'warning');
                return;
            }
            
            const a = parseFloat(aValue);
            const b = parseFloat(bValue);
            
            // Check if parsing resulted in valid numbers
            if (isNaN(a) || isNaN(b)) {
                showCalcResult(`Invalid input - First: "${aValue}", Second: "${bValue}". Please enter valid numbers.`, 'warning');
                return;
            }

            // Disable button and show loading state
            testCalcBtn.disabled = true;
            testCalcBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Calculating...';
            
            try {
                const requestBody = {
                    method: 'tools/call',
                    name: 'calculator',
                    arguments: {
                        operation: operation,
                        a: a,
                        b: b
                    }
                };
                
                console.log('Sending request:', requestBody);
                
                const response = await fetch('/api/mcp/tools/call', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(requestBody)
                });

                const data = await response.json();
                console.log('Response received:', data);
                
                if (response.ok && data.result) {
                    const content = data.result.content;
                    if (content && content.length > 0) {
                        showCalcResult(`✅ ${content[0].text}`, 'success');
                    } else {
                        showCalcResult('Calculator tool returned empty result.', 'warning');
                    }
                } else if (data.error) {
                    showCalcResult(`❌ Error: ${data.error.message}`, 'danger');
                } else {
                    showCalcResult('❌ Unexpected response format.', 'danger');
                }
            } catch (error) {
                console.error('Calculator error:', error);
                showCalcResult(`❌ Network error: ${error.message}`, 'danger');
            } finally {
                // Re-enable button
                testCalcBtn.disabled = false;
                testCalcBtn.innerHTML = 'Calculate';
            }
        });

        // Allow Enter key to trigger calculation from number inputs
        [calcA, calcB].forEach(input => {
            if (input) {
                input.addEventListener('keypress', function(e) {
                    if (e.key === 'Enter') {
                        testCalcBtn.click();
                    }
                });
            }
        });
    } else {
        // Debug: log which elements are missing
        console.log('Calculator elements check:');
        console.log('testCalcBtn:', testCalcBtn);
        console.log('calcResult:', calcResult);
        console.log('calcA:', calcA);
        console.log('calcOperation:', calcOperation);
        console.log('calcB:', calcB);
    }

    // Word Counter functionality
    if (testWordCountBtn && wordCountResult) {
        testWordCountBtn.addEventListener('click', async function() {
            // Get fresh references to ensure elements exist
            const textElement = document.getElementById('wordCountText');
            const spacesElement = document.getElementById('includeSpaces');
            
            if (!textElement || !spacesElement) {
                showWordCountResult('Word counter form elements not found. Please refresh the page.', 'danger');
                return;
            }
            
            const text = textElement.value;
            const includeSpacesValue = spacesElement.checked;
            
            // Debug logging
            console.log('Word counter inputs:', { text: text.substring(0, 50) + '...', includeSpaces: includeSpacesValue });
            
            // Check if text exists
            if (text.trim() === '') {
                showWordCountResult('Please enter some text to analyze.', 'warning');
                return;
            }

            // Disable button and show loading state
            testWordCountBtn.disabled = true;
            testWordCountBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Analyzing...';
            
            try {
                const requestBody = {
                    method: 'tools/call',
                    name: 'word_counter',
                    arguments: {
                        text: text,
                        include_spaces: includeSpacesValue
                    }
                };
                
                console.log('Sending word counter request:', requestBody);
                
                const response = await fetch('/api/mcp/tools/call', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(requestBody)
                });

                const data = await response.json();
                console.log('Word counter response received:', data);
                
                if (response.ok && data.result) {
                    const content = data.result.content;
                    if (content && content.length > 0) {
                        showWordCountResult(`✅ ${content[0].text}`, 'success');
                    } else {
                        showWordCountResult('Word counter tool returned empty result.', 'warning');
                    }
                } else if (data.error) {
                    showWordCountResult(`❌ Error: ${data.error.message}`, 'danger');
                } else {
                    showWordCountResult('❌ Unexpected response format.', 'danger');
                }
            } catch (error) {
                console.error('Word counter error:', error);
                showWordCountResult(`❌ Network error: ${error.message}`, 'danger');
            } finally {
                // Re-enable button
                testWordCountBtn.disabled = false;
                testWordCountBtn.innerHTML = 'Analyze Text';
            }
        });

        // Allow Enter key to trigger word count from textarea (Ctrl+Enter)
        if (wordCountText) {
            wordCountText.addEventListener('keydown', function(e) {
                if (e.key === 'Enter' && e.ctrlKey) {
                    testWordCountBtn.click();
                }
            });
        }
    } else {
        // Debug: log which elements are missing
        console.log('Word counter elements check:');
        console.log('testWordCountBtn:', testWordCountBtn);
        console.log('wordCountResult:', wordCountResult);
        console.log('wordCountText:', wordCountText);
        console.log('includeSpaces:', includeSpaces);
    }

    function showResult(message, type) {
        echoResult.className = `alert alert-${type}`;
        echoResult.textContent = message;
        echoResult.style.display = 'block';
        
        // Auto-hide success messages after 5 seconds
        if (type === 'success') {
            setTimeout(() => {
                echoResult.style.display = 'none';
            }, 5000);
        }
    }

    function showToolsResult(message, type) {
        toolsList.className = `alert alert-${type}`;
        toolsList.innerHTML = message;
        toolsList.style.display = 'block';
        
        // Auto-hide success messages after 10 seconds
        if (type === 'success') {
            setTimeout(() => {
                toolsList.style.display = 'none';
            }, 10000);
        }
    }

    function showCalcResult(message, type) {
        calcResult.className = `alert alert-${type}`;
        calcResult.textContent = message;
        calcResult.style.display = 'block';
        
        // Auto-hide success messages after 5 seconds
        if (type === 'success') {
            setTimeout(() => {
                calcResult.style.display = 'none';
            }, 5000);
        }
    }

    function showWordCountResult(message, type) {
        wordCountResult.className = `alert alert-${type}`;
        // Use innerHTML for word counter to preserve line breaks
        wordCountResult.innerHTML = message.replace(/\n/g, '<br>');
        wordCountResult.style.display = 'block';
        
        // Auto-hide success messages after 10 seconds (longer for word count results)
        if (type === 'success') {
            setTimeout(() => {
                wordCountResult.style.display = 'none';
            }, 10000);
        }
    }

    // Resources loading functionality
    const listResourcesBtn = document.getElementById('listResourcesBtn');
    const resourcesList = document.getElementById('resourcesList');
    const resourcesContainer = document.getElementById('resourcesContainer');
    const resourcesError = document.getElementById('resourcesError');

    if (listResourcesBtn) {
        listResourcesBtn.addEventListener('click', async function() {
            // Disable button and show loading state
            listResourcesBtn.disabled = true;
            listResourcesBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Loading...';
            
            // Hide previous results/errors
            resourcesList.style.display = 'none';
            resourcesError.style.display = 'none';
            
            try {
                const response = await fetch('/api/mcp/resources/list', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        method: 'resources/list'
                    })
                });

                const data = await response.json();

                if (response.ok && data.result && data.result.resources) {
                    displayResources(data.result.resources);
                } else {
                    throw new Error(data.error?.message || 'Failed to load resources');
                }
            } catch (error) {
                console.error('Error loading resources:', error);
                showResourcesError(`Error: ${error.message}`);
            } finally {
                // Re-enable button and restore text
                listResourcesBtn.disabled = false;
                listResourcesBtn.innerHTML = 'Load Available Resources';
            }
        });
    }

    function displayResources(resources) {
        resourcesContainer.innerHTML = '';
        
        if (!resources || resources.length === 0) {
            resourcesContainer.innerHTML = '<p class="text-muted">No resources available.</p>';
        } else {
            resources.forEach((resource, index) => {
                const resourceElement = createResourceElement(resource, index);
                resourcesContainer.appendChild(resourceElement);
            });
        }
        
        resourcesList.style.display = 'block';
    }

    function createResourceElement(resource, index) {
        const div = document.createElement('div');
        
        // Extract filename from URI (e.g., file:///sample.txt -> sample.txt)
        const filename = resource.uri.split('/').pop();
        
        // Determine file type and icon
        const isJson = filename.endsWith('.json');
        const icon = isJson ? '📊' : '📄';
        const buttonClass = isJson ? 'btn-outline-success' : 'btn-outline-primary';
        
        // Create download URL
        const downloadUrl = `/api/mcp/resources/download/${filename}`;
        
        div.innerHTML = `
            <a href="${downloadUrl}" class="btn ${buttonClass}" download="${filename}">
                ${icon} Download ${filename}
                <small class="d-block text-muted">${resource.description || 'MCP resource file'}</small>
                <small class="d-block text-muted" style="font-size: 0.75em;">URI: ${resource.uri}</small>
            </a>
        `;
        
        return div;
    }

    function showResourcesError(message) {
        resourcesError.textContent = message;
        resourcesError.style.display = 'block';
    }
});
