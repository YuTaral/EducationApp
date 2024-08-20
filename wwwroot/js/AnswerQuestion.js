const NUMERIC = "Numeric";
const TEXT = "Text";
const MULTIPLE_ANSWER = "Multiple answer";
const SINGLE_ANSWER = "Single answer";


function populateSelectableQuestions() {
    var question = document.querySelectorAll('.selectable-question')[0];

    if (question != undefined) {
        var typeAndPossibleAns = question.querySelectorAll('input');
        var type = typeAndPossibleAns[0];

        var answersContainers = document.querySelectorAll('.answers-container')[0];
        var possibleAnswers = typeAndPossibleAns[1].value.split('|');
        var answerOptionId = type.value == SINGLE_ANSWER ? 'answerOptionSingle' : 'answerOptionMulti';
        var answerOption = document.getElementById(answerOptionId);

        for (j = 0; j < possibleAnswers.length; j++) {
            var cloned = answerOption.cloneNode(true);
            var span = cloned.querySelectorAll('span')[0];
            var answerId = cloned.querySelectorAll('.answer-id')[0];
            var idAndText = possibleAnswers[j].split("_");
            var id = idAndText[0];
            var text = idAndText[1];

            answerId.value = id;
            span.innerText = text;
            cloned.id = '';
            cloned.style.display = '';
            answersContainers.appendChild(cloned);
        }

        populateAnsweredSelectableQuestion(question);
    }
}

// Populate answered question
function populateAnsweredSelectableQuestion(question) {
    var type = question.querySelectorAll('input')[0].value;
    var questionAnswers = document.getElementById('questionAnswers');
    var possibleAnswersClassName = type == SINGLE_ANSWER ? '.possible-answer-single' : '.possible-answer';
    var answersContainer = document.querySelectorAll(possibleAnswersClassName);
    var answer = questionAnswers.value.split('|||');

    if (type == SINGLE_ANSWER) {
        for (i = 0; i < answersContainer.length - 1; i++) {
            var answerId = answersContainer[i].querySelectorAll('input')[0].value;
            var selected = answersContainer[i].querySelectorAll('input')[1];

            if (answer == answerId) {
                selected.checked = true;
            }
        }
    } else {
        var answers = answer[0].split(',');

        for (i = 0; i < answersContainer.length - 1; i++) {
            var answerId = answersContainer[i].querySelectorAll('input')[0].value;
            var selected = answersContainer[i].querySelectorAll('input')[1];

            if (answers.includes(answerId)) {
                selected.checked = true;
            }
        }

    }

    
}


// Form submission listener
function handleSubmit() {
    var question = document.querySelectorAll('.selectable-question')[0];

    if (question != undefined) {
        var type = question.querySelectorAll('input')[0];

        var questionAnswers = document.getElementById('questionAnswers');
        questionAnswers.value = '';
        var possibleAnswersClassName = type.value == "Single answer" ? '.possible-answer-single' : '.possible-answer';
        var possibleAnswers = document.querySelectorAll(possibleAnswersClassName);

        for (i = 0; i < possibleAnswers.length - 1; i++) {
            var isChecked = possibleAnswers[i].querySelectorAll('input')[1].checked;

            if (isChecked) {
                var answerId = possibleAnswers[i].querySelectorAll('.answer-id')[0].value;
                questionAnswers.value = questionAnswers.value + answerId + ',';
            }
        }
    }

    if (questionAnswers.value.length > 1) {
        questionAnswers.value = questionAnswers.value.slice(0, -1);
    }
    return true;
}


populateSelectableQuestions();