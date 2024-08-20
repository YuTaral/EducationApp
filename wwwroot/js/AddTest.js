window.onload = () => {
    generateQuestionFields(false);

    if (document.getElementById('editQuestionField').value == 'True') {
        populateQuestionWhenEdit();
    }
};

const NUMERIC = "Numeric";
const TEXT = "Text";
const MULTIPLE_ANSWER = "Multiple answer";
const SINGLE_ANSWER = "Single answer";

// Question type changed
function generateQuestionFields(removePrevious) {
    var qTypeSelect = document.getElementById('questionTypeSelect');
    var selectedOption = qTypeSelect.options[qTypeSelect.selectedIndex].value;
    var singleMultiAnswersContainer = document.getElementById('singleMultiAnswersContainer');
    var count = document.getElementById('answersCount').value;

    switch (selectedOption) {
        case NUMERIC:
            singleMultiAnswersContainer.style.display = 'none';
            removePreviousAnsCont();
            break;

        case TEXT:
            singleMultiAnswersContainer.style.display = 'none';
            removePreviousAnsCont();
            break;

        case SINGLE_ANSWER:
            singleMultiAnswersContainer.style.display = '';
            manageAnswersCount(count, true, removePrevious, true);
            break;

        case MULTIPLE_ANSWER:
            singleMultiAnswersContainer.style.display = '';
            manageAnswersCount(count, false, removePrevious, true);
            break;
    }
}


function manageAnswersCount(count, isSingle, removePrevious, adding) {
    var answerOptionId = isSingle ? 'answerOptionSingle' : 'answerOptionMulti';
    var container = document.getElementById('answersContainer');
    var answerOption = document.getElementById(answerOptionId);
    var singleMultiAnswersContainer = document.getElementById('singleMultiAnswersContainer');

    if (adding) {

        if (removePrevious) {
            removePreviousAnsCont()
        }

        for (var i = 0; i < count; i++) {
            var cloned = answerOption.cloneNode(true);
            cloned.id = '';
            cloned.style.display = '';
            container.appendChild(cloned);
        }

        container.style.display = '';
        singleMultiAnswersContainer.appendChild(container);

    } else {

        var stopAtIndex = container.childNodes.length - 1 - count;

        if (stopAtIndex < 1) {
            stopAtIndex = 1;
        }

        for (var i = container.childNodes.length - 1; i > stopAtIndex; i--) {
            container.removeChild(container.children[i]);
        }
    }

}


function addRemoveAnswers(adding) {
    var qTypeSelect = document.getElementById('questionTypeSelect');
    var qType = qTypeSelect.options[qTypeSelect.selectedIndex].value;
    var count = document.getElementById('answersCount').value;

    if (adding) {
        switch (qType) {
            case MULTIPLE_ANSWER:
                manageAnswersCount(count, false, false, true)
                break;

            case SINGLE_ANSWER:
                manageAnswersCount(count, true, false, true)
                break;
        }

    } else {
        switch (qType) {
            case MULTIPLE_ANSWER:
                manageAnswersCount(count, false, false, false)
                break;

            case SINGLE_ANSWER:
                manageAnswersCount(count, true, false, false)
                break;
        }
    }
}

function removePreviousAnsCont() {
    var containerToRemove = document.getElementById('answersContainer');

    if (containerToRemove) {
        containerToRemove.innerHTML = '';
        var mainContainer = document.querySelector('.container');
        mainContainer.appendChild(containerToRemove);
    }
}


// Form submission listener
function handleSubmit() {
    var qTypeSelect = document.getElementById('questionTypeSelect');
    var typeField = document.getElementById('typeField');
    typeField.value = qTypeSelect.options[qTypeSelect.selectedIndex].value;

    // Populate answers field before submission if creating multi/single question
    if (typeField.value == 'Single answer' || typeField.value == 'Multiple answer') {
        var questionAnswers = document.getElementById('questionAnswers');
        questionAnswers.value = '';
        var possibleAnswersClassName = typeField.value == "Single answer" ? '.possible-answer-single' : '.possible-answer';
        var possibleAnswers = document.querySelectorAll(possibleAnswersClassName);
        var correctAnswersCounter = 0;

        for (i = 0; i < possibleAnswers.length - 1; i++) {
            var isCorrect = possibleAnswers[i].querySelectorAll('input')[0].checked;
            var answer = possibleAnswers[i].querySelectorAll('input')[1];
            var invalidAnswer = false;

            if (answer.value == '') {
                invalidAnswer = true;
                showInvalidAnswer(answer, 'Please enter answer text...');

            } else {
                answer.style.borderColor = '';
                answer.placeholder = 'Answer\'s text...';
                var answerText = answer.value;

                if (answerText.includes('#') || answerText.includes('_') || answerText.includes('|')) {
                    invalidAnswer = true;
                    showInvalidAnswer(answer, '#,_,| are forbidden');
                }
                
                
                if (isCorrect) {
                    correctAnswersCounter += 1;
                }

                questionAnswers.value = questionAnswers.value + isCorrect + '_' + answerText + '|';
            }
        }

        if (invalidAnswer) {
            questionAnswers.value = '';
            return false;
        }

        if (correctAnswersCounter == 0) {
            showModal();
            questionAnswers.value = '';
            return false;
        }

    }

    if (typeField.value == "Multiple answer" && correctAnswersCounter == 1) {
        typeField.value = "Single answer"
    }

    if (questionAnswers.value.length > 1) {
        questionAnswers.value = questionAnswers.value.slice(0, questionAnswers.value.length - 1);
    }

    return true;
}


function showInvalidAnswer(answer, errorText) {
    answer.style.border = 'solid';
    answer.style.borderColor = 'black';
    answer.value = '';
    answer.placeholder = errorText;
}


// Show modal
function showModal() {
    var modal = document.getElementById('modalResults');
    var closeBtn = document.getElementById('closeResults');

    modal.style.display = 'block';

    closeBtn.addEventListener('click', (e) => {
        modal.style.display = 'none';
    });
}


// Populate question when edit
function populateQuestionWhenEdit() {
    let qTypeSelect = document.getElementById('questionTypeSelect');
    var typeValue = document.getElementById('typeField').value;

    for (let i = 0; i < qTypeSelect.options.length; i++) {
        if (qTypeSelect.options[i].value == typeValue) {
            qTypeSelect.selectedIndex = i;
        }
    }

    qTypeSelect.disabled = true;

    if (typeValue == "Text" || typeValue == "Numeric") {
        document.getElementById('singleMultiAnswersContainer').style.display = 'none';

    } else {

        if (typeValue == "Single answer") {
            var count = document.getElementById('answersCount').value;
            manageAnswersCount(count, true, true, true)
        }

        var questionAnswers = document.getElementById('questionAnswers');
        var possibleAnswersClassName = typeValue == "Single answer" ? '.possible-answer-single' : '.possible-answer';
        var answersContainer = document.querySelectorAll(possibleAnswersClassName);
        var answers = questionAnswers.value.split('|');

        for (i = 0; i < answersContainer.length - 1; i++) {
            var isCorrectField = answersContainer[i].querySelectorAll('input')[0];
            var answerField = answersContainer[i].querySelectorAll('input')[1];
            var isCorrectAnswerText = answers[i].split('_');
            var isCorrect = isCorrectAnswerText[0] == 'True';
            var answerText = isCorrectAnswerText[1];

            if (isCorrect) {
                isCorrectField.checked = true;
            }
            answerField.value = answerText;
        }
    }
}